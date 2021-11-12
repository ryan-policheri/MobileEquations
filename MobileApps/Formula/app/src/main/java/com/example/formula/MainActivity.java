package com.example.formula;

import androidx.appcompat.app.AppCompatActivity;

import android.content.ActivityNotFoundException;
import android.content.Intent;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.View;
import android.widget.ImageView;
import android.widget.TextView;

import com.agog.mathdisplay.MTMathView;

import java.io.IOException;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

import edu.uiowa.common.webapiclient.CustomHttpClient;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        String default_text = "x = \\frac{-b \\pm \\sqrt{b^2-4ac}}{2a}";
        MTMathView mathview = (MTMathView) this.findViewById(R.id.mathview);
        TextView latex_text = (TextView) this.findViewById(R.id.description);
        latex_text.setText(default_text);
        mathview.setFontSize(100);
        mathview.setLatex(default_text);
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

    public void pingApi(View v) throws IOException, InterruptedException, ExecutionException {
        ExecutorService executor = Executors.newFixedThreadPool(10);
        Future<Boolean> future = executor.submit(new Callable<Boolean>(){
            @Override
            public Boolean call() throws Exception {
                CustomHttpClient client = new CustomHttpClient("http://52.204.181.13:9000/api/");
                Boolean result = client.get(Boolean.class, "equations/ping");
                return result;
            }
        });
        Boolean result = future.get();
        System.out.println(result);
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
        }
    }
}