/*
 * Copyright 2014 Niek Haarman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//package com.haarman.listviewanimations;

//import android.annotation.SuppressLint;
//import android.app.Activity;
//import android.os.Build;
//import android.os.Bundle;
//import android.view.MenuItem;
//import android.view.WindowManager;
using Android.App;
using Android.OS;
using Android.Views;
namespace ListviewAnimations.Sample
{
    public class BaseActivity : Activity
    {

        //@SuppressLint("InlinedApi")
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                //getWindow().addFlags(WindowManager.LayoutParams.FLAG_TRANSLUCENT_NAVIGATION);
                Window.AddFlags(WindowManagerFlags.TranslucentNavigation);  
            }
            base.OnCreate(savedInstanceState);

            //assert getActionBar() != null;
            //getActionBar().setDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        //@Override
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}