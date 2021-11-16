package com.example.formula;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.os.HandlerCompat;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.provider.MediaStore;
import android.provider.Settings;
import android.util.Base64;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.agog.mathdisplay.MTMathView;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.util.UUID;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import edu.uiowa.formula.model.ClientInfo;
import edu.uiowa.formula.model.Equation;
import edu.uiowa.formula.services.EquationService;

public class MainActivity extends AppCompatActivity {
    protected final ExecutorService _executer;
    protected final Handler _mainThreadHandler;

    private EquationService _service;
    private int _pingCounter;
    private int _testPostCounter;

    public MainActivity() {
        _executer = Executors.newFixedThreadPool(4);
        _mainThreadHandler = HandlerCompat.createAsync(Looper.getMainLooper());
        _pingCounter = 0;
        _testPostCounter = 0;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        String url = this.getString(R.string.base_url);
        _service = new EquationService(url); //TODO: Use Factory

        setContentView(R.layout.activity_main);
        String default_text = "x = \\frac{-b \\pm \\sqrt{b^2-4ac}}{2a}";
        MTMathView mathview = (MTMathView) this.findViewById(R.id.mathview);
        TextView latex_text = (TextView) this.findViewById(R.id.description);
        latex_text.setText(default_text);
        mathview.setFontSize(100);
        mathview.setLatex(default_text);
    }

    public void pingApi(View view) throws IOException, InterruptedException, ExecutionException {
        RunnableWithCallback action = new RunnableWithCallback(_mainThreadHandler) {
            @Override
            public void run() {
                try {
                    Boolean result = _service.pingController();
                    this.postCallback(new Runnable() {
                        @Override
                        public void run() {
                            if(result) _pingCounter++;
                            TextView pingText = (TextView) findViewById(R.id.textPing);
                            pingText.setText("Ping: " + _pingCounter);
                        }
                    });
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        };

        _executer.submit(action);
    }

    public void testPost(View view) throws IOException, InterruptedException {
        Equation equation = new Equation(buildClientInfo());

        RunnableWithCallback action = new RunnableWithCallback(_mainThreadHandler) {
            @Override
            public void run() {
                try {
                    Equation solvedEquation = _service.solveEquation(equation);
                    this.postCallback(new Runnable() {
                        @Override
                        public void run() {
                            if(solvedEquation != null) _testPostCounter++;
                            TextView pingText = (TextView) findViewById(R.id.textTestPost);
                            pingText.setText("Post: " + _testPostCounter);
                        }
                    });
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        };

        _executer.submit(action);
    }

    static final int REQUEST_IMAGE_CAPTURE = 1;

    private void dispatchTakePictureIntent() {
        Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        try {
            startActivityForResult(takePictureIntent, REQUEST_IMAGE_CAPTURE);
        } catch (ActivityNotFoundException e) {
            // display error state to the user
        }
    }
    public void buttonStuff(View v) {
        dispatchTakePictureIntent();
        updateLatex("x = \\alpha^{\\sum}");
    }

    private void updateLatex(String text) {
        TextView latex_text = (TextView) this.findViewById(R.id.description);
        latex_text.setText(text);
        MTMathView mathview = (MTMathView) this.findViewById(R.id.mathview);
        mathview.setLatex(text);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == REQUEST_IMAGE_CAPTURE && resultCode == RESULT_OK) {
            Bundle extras = data.getExtras();
            Bitmap imageBitmap = (Bitmap) extras.get("data");
            ImageView imageView = (ImageView) this.findViewById(R.id.imageView);
            imageView.setImageBitmap(imageBitmap);

            Equation equation = new Equation(buildClientInfo());
            equation = _service.solveEquation(equation, imageBitmap);
        }
    }

    private ClientInfo buildClientInfo(){
        String androidId = Settings.Secure.getString(this.getContentResolver(), Settings.Secure.ANDROID_ID);
        return new ClientInfo(androidId, Build.VERSION.BASE_OS, Build.VERSION.SDK_INT, Build.VERSION.RELEASE);
    }
}