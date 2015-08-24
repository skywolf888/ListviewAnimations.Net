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

//package com.haarman.listviewanimations.gridview;

//import android.os.Bundle;
//import android.widget.GridView;

//import com.haarman.listviewanimations.BaseActivity;
//import com.haarman.listviewanimations.R;
//import com.nhaarman.listviewanimations.appearance.simple.SwingBottomInAnimationAdapter;

using Android.App;
using Android.OS;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Appearance.Simple;
namespace ListviewAnimations.Sample.gridview
{
    [Activity(Label = "GridViewActivity(.Net)") ]
    public class GridViewActivity : BaseActivity
    {

        private static readonly int INITIAL_DELAY_MILLIS = 300;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_gridview);
			
            GridView gridView = (GridView)FindViewById(Resource.Id.activity_gridview_gv);
            SwingBottomInAnimationAdapter swingBottomInAnimationAdapter = new SwingBottomInAnimationAdapter(new GridViewAdapter(this));
            swingBottomInAnimationAdapter.setAbsListView(gridView);

            //assert swingBottomInAnimationAdapter.getViewAnimator() != null;
            swingBottomInAnimationAdapter.getViewAnimator().setInitialDelayMillis(INITIAL_DELAY_MILLIS);

            gridView.Adapter = swingBottomInAnimationAdapter;

            //assert getActionBar() != null;
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }
    }
}
