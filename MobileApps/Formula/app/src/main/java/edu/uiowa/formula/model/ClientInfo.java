package edu.uiowa.formula.model;

import com.alibaba.fastjson.annotation.JSONField;

public class ClientInfo {
    @JSONField(name="deviceId")
    private String _deviceId;
    @JSONField(name="operatingSystem")
    private String _operatingSystem;
    @JSONField(name="sdkVersion")
    private int _sdkVersion;
    @JSONField(name="sdkRelease")
    private String _sdkRelease;

    public ClientInfo() {
    }

    public ClientInfo(String deviceId, String operatingSystem, int sdkVersion, String sdkRelease){
        _deviceId = deviceId;
        _operatingSystem = operatingSystem;
        _sdkVersion = sdkVersion;
        _sdkRelease = sdkRelease;
    }

    public String get_deviceId() {
        return _deviceId;
    }

    public void set_deviceId(String _deviceId) {
        this._deviceId = _deviceId;
    }

    public String get_operatingSystem() {
        return _operatingSystem;
    }

    public void set_operatingSystem(String _operatingSystem) {
        this._operatingSystem = _operatingSystem;
    }

    public int get_sdkVersion() {
        return _sdkVersion;
    }

    public void set_sdkVersion(int _sdkVersion) {
        this._sdkVersion = _sdkVersion;
    }

    public String get_sdkRelease() {
        return _sdkRelease;
    }

    public void set_sdkRelease(String _sdkRelease) {
        this._sdkRelease = _sdkRelease;
    }
}
