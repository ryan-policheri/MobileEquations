package edu.uiowa.formula.model;

import com.alibaba.fastjson.annotation.JSONField;

public class ProcessedEquation {
    @JSONField(name="equation")
    private String _equation;
    @JSONField(name="solution")
    private String _solution;
    @JSONField(name="solvedEquation")
    private String _solvedEquation;
    @JSONField(name="laTeX")
    private String _laTex;

    public ProcessedEquation() {
    }

    public String get_equation() {
        return _equation;
    }

    public void set_equation(String _equation) {
        this._equation = _equation;
    }

    public String get_solution() {
        return _solution;
    }

    public void set_solution(String _solution) {
        this._solution = _solution;
    }

    public String get_solvedEquation() {
        return _solvedEquation;
    }

    public void set_solvedEquation(String _solvedEquation) {
        this._solvedEquation = _solvedEquation;
    }

    public String get_laTex() {
        return _laTex;
    }

    public void set_laTex(String _laTex) {
        this._laTex = _laTex;
    }
}
