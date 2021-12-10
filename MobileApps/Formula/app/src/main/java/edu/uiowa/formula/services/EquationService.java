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

    public Equation solveEquation(Equation equation, Bitmap picture, String fileDir) throws IOException, InterruptedException {
        ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
        picture.compress(Bitmap.CompressFormat.JPEG, 100, outputStream);
        byte[] bytes = outputStream.toByteArray();

        String uniqueFileName = fileDir + "/" + UUID.randomUUID().toString() + ".jpg";
        File file = new File(uniqueFileName);
        file.createNewFile();
        FileOutputStream fileOutputStream = new FileOutputStream(file);
        fileOutputStream.write(bytes);
        fileOutputStream.close();

        return _client.post(Equation.class, "", equation, file);
    }
}
