package edu.uiowa.formula.services;

import android.graphics.Bitmap;

import org.apache.commons.lang3.StringUtils;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.UUID;

import edu.uiowa.common.webapiclient.CustomHttpClient;
import edu.uiowa.formula.model.Equation;

public class EquationService {
    private final String _equationsController = "/equations";
    private CustomHttpClient _client;

    public EquationService(String baseUrl) {
        baseUrl = StringUtils.stripEnd(baseUrl, "/") + _equationsController;
        _client = new CustomHttpClient(baseUrl);
    }

    public boolean pingController() throws IOException, InterruptedException {
        return _client.get(Boolean.class, "ping");
    }

    public Equation solveEquation(Equation equation) throws IOException, InterruptedException {
        Equation solution = _client.post(Equation.class, "testsimplepost", equation);
        return solution;
    }

    public Equation solveEquation(Equation equation, Bitmap picture, String fileDir) {
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        picture.compress(Bitmap.CompressFormat.PNG, 100, outputStream);
        byte[] bytes = outputStream.toByteArray();

        try {
            String uniqueFileName = fileDir + "/" + UUID.randomUUID().toString() + ".png";
            File file = new File(uniqueFileName);
            file.createNewFile();
            FileOutputStream fileOutputStream = new FileOutputStream(file);
            fileOutputStream.write(bytes);
            fileOutputStream.close();

            return _client.post(Equation.class, "", equation, file);
        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException | InterruptedException e) {
            e.printStackTrace();
        }

        return null;
    }
}