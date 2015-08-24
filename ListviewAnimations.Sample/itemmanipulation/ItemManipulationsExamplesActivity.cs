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
//package com.haarman.listviewanimations.itemmanipulation;

//import android.annotation.SuppressLint;
//import android.content.Intent;
//import android.os.Bundle;
//import android.view.View;

//import com.haarman.listviewanimations.BaseActivity;
//import com.haarman.listviewanimations.R;
//import com.haarman.listviewanimations.itemmanipulation.expandablelistitems.ExpandableListItemActivity;


using Android.OS;
using Android.Views;
using Android.Content;
using ListviewAnimations.Sample.itemmanipulation.expandablelistitems;
using Android.App;
using Android.Widget;

namespace ListviewAnimations.Sample.itemmanipulation
{

    [Activity(Label = "ItemManipulationsExamplesActivity(.Net)")]
    public class ItemManipulationsExamplesActivity : BaseActivity
    {

        //@SuppressLint("InlinedApi")
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_examples_itemmanipulations);
            FrameLayout flExpand = (FrameLayout)FindViewById(Resource.Id.ExpandList);
            flExpand.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ExpandableListItemActivity));
                StartActivity(intent);
            };
            FrameLayout flDyn = (FrameLayout)FindViewById(Resource.Id.DynamicListView);
            flDyn.Click += delegate
            {
                Intent intent = new Intent(this, typeof(DynamicListViewActivity));
                StartActivity(intent);
            };
        }

        public void onDynamicListViewClicked(View view)
        {
            Intent intent = new Intent(this, typeof(DynamicListViewActivity));
            StartActivity(intent);
        }

        public void onExpandListItemAdapterClicked(View view)
        {
            Intent intent = new Intent(this, typeof(ExpandableListItemActivity));
            StartActivity(intent);
        }
    }
}