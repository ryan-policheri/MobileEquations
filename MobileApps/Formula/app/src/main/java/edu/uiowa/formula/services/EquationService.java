package edu.uiowa.formula.services;

import org.apache.commons.lang3.StringUtils;

import java.io.IOException;

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
}
