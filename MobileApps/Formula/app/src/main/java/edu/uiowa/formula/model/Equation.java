package edu.uiowa.formula.model;

import com.alibaba.fastjson.annotation.JSONField;

public class Equation {

    @JSONField(name="clientInfo")
    private ClientInfo _clientInfo;

    @JSONField(name="processedEquation")
    private ProcessedEquation _processedEquation;

    public Equation() {

    }

    public Equation(ClientInfo clientInfo) {
        _clientInfo = clientInfo;
    }

    public ClientInfo get_clientInfo() {
        return _clientInfo;
    }

    public void set_clientInfo(ClientInfo _client) {
        this._clientInfo = _client;
    }

    public ProcessedEquation get_processedEquation() {
        return _processedEquation;
    }

    public void set_processedEquation(ProcessedEquation _processedEquation) {
        this._processedEquation = _processedEquation;
    }
}
