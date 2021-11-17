package com.example.formula;

import android.os.Handler;

public class RunnableWithCallback implements Runnable {
    private final Handler _mainThreadHandler;

    public RunnableWithCallback(Handler mainThreadHandler){
        _mainThreadHandler = mainThreadHandler;
    }

    @Override
    public void run() {

    }

    public void postCallback(Runnable callback){
        _mainThreadHandler.post(callback);
    }
}
