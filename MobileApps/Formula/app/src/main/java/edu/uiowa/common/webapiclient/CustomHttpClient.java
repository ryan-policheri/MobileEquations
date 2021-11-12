package edu.uiowa.common.webapiclient;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.TypeReference;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
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

    private String readResponse(HttpURLConnection connection) throws IOException {
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
        if (_baseUrl != null) {
            URL url = new URL(fullOrPartialUrl); //assume full
            return url;
        }
        else {
            URL url = new URL(StringUtils.stripEnd(_baseUrl, "/") + "/" + StringUtils.stripStart(fullOrPartialUrl, "/"));
            return url;
        }
    }
}
