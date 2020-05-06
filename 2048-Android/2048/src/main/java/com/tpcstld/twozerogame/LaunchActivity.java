package com.tpcstld.twozerogame;

import android.os.Bundle;

import com.skillz.Skillz;
import com.skillz.SkillzActivity;

public class LaunchActivity extends SkillzActivity {
    @Override
    public void onCreate(Bundle savedInstance) {
        super.onCreate(savedInstance);

        setContentView(R.layout.skillz_activity);
        Skillz.launch(this);
    }
}
