package edu.uiowa.common.webapiclient;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.TypeReference;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.Map;

import org.apache.commons.lang3.StringUtils;

public class CustomHttpClient {
    private final String GET = "GET";
    private final String POST = "POST";

    private String _baseUrl;
    private int _retryCount = 0;

    public CustomHttpClient() {
        _baseUrl = null;
    }

    public CustomHttpClient(String baseUrl) {
        if (baseUrl == null || baseUrl.trim().equals("")) throw new IllegalArgumentException("base url cannot be null, use other constructor");
        _baseUrl = baseUrl;
    }

    public <T> T get(Class<T> generic, String route) throws IOException, InterruptedException {
        HttpURLConnection connection = prepareConnection(route, GET);
        String response = readResponse(connection);
        T obj = JSON.parseObject(response, generic);
        return obj;
    }

    public <T> T post(Class<T> generic, String route, Object payload) throws IOException, InterruptedException {
        HttpURLConnection connection = prepareConnection(route, POST);
        connection.setRequestProperty("Content-Type", "application/json; utf-8");
        connection.setRequestProperty("Accept", "application/json");
        writePayload(connection, payload);
        String response = readResponse(connection);
        T obj = JSON.parseObject(response, generic);
        return obj;
    }

    public <T> T post(Class<T> generic, String route, Object payload, File file) throws IOException, InterruptedException {
        try {
            Map<String, String> headers = new HashMap<>();
            String url = prepareUrl(route).toString();
            MultipartUtility multipart = new MultipartUtility(url, "utf-8", headers);
            multipart.addFormField("jsonString", JSON.toJSONString(payload));
            multipart.addFilePart("file", file);
            String response = multipart.finish();
            T obj = JSON.parseObject(response, generic);
            return obj;
        }
        catch (Exception ex) {
            if (_retryCount > 3) throw ex;
            else {
                _retryCount++;
                Thread.sleep(100);
                return post(generic, route, payload, file);
            }
        }
    }

    private void writePayload(HttpURLConnection connection, Object payload) throws IOException, InterruptedException {
        connection.setDoOutput(true);
        try(DataOutputStream writer = new DataOutputStream( connection.getOutputStream()))
        {
            String json = JSON.toJSONString(payload);
            byte[] bytes = json.getBytes(StandardCharsets.UTF_8 );
            writer.write(bytes);
        }
        catch (IOException ex) {
            if (_retryCount > 3) throw ex;
            else {
                _retryCount++;
                Thread.sleep(100);
                writePayload(connection, payload);
            }
        }
    }

    private String readResponse(HttpURLConnection connection) throws IOException, InterruptedException {
        try {
            InputStream inputStream = connection.getInputStream();
            BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));
            StringBuilder response = new StringBuilder();
            String line;
            while ((line = reader.readLine()) != null) {
                response.append(line);
                response.append('\r');
            }
            reader.close();
            return response.toString();
        }
        catch (IOException ex) {
            if (_retryCount > 3) throw ex;
            else {
                _retryCount++;
                Thread.sleep(100);
                return readResponse(connection);
            }
        }
    }

    private HttpURLConnection prepareConnection(String route, String method) throws IOException, InterruptedException {
        URL url = prepareUrl(route);
        try {
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod(method);
            return connection;
        }
        catch (IOException ex) {
            if (_retryCount > 3) throw ex;
            else {
                _retryCount++;
                Thread.sleep(100);
                return prepareConnection(route, method);
            }
        }
    }

    private URL prepareUrl(String fullOrPartialUrl) throws MalformedURLException {
        if (_baseUrl == null) {
            URL url = new URL(fullOrPartialUrl); //assume full
            return url;
        }
        else {
            String urlString = StringUtils.stripEnd(_baseUrl, "/") + "/" + StringUtils.stripStart(fullOrPartialUrl, "/");
            urlString = StringUtils.strip(urlString, "/");
            URL url = new URL(urlString);
            return url;
        }
    }
}
