package edu.uiowa.formula.model;

import com.alibaba.fastjson.annotation.JSONField;

public class Equation {

    @JSONField(name="client")
    private String _client;

    @JSONField(name="processedEquation")
    private ProcessedEquation _processedEquation;

    public Equation() {

    }

    public Equation(String client) {
        _client = client;
    }

    public String get_client() {
        return _client;
    }

    public void set_client(String _client) {
        this._client = _client;
    }

    public ProcessedEquation get_processedEquation() {
        return _processedEquation;
    }

    public void set_processedEquation(ProcessedEquation _processedEquation) {
        this._processedEquation = _processedEquation;
    }
}
