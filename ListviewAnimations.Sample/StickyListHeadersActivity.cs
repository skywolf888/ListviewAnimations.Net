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

//import android.os.Bundle;

//import com.nhaarman.listviewanimations.appearance.StickyListHeadersAdapterDecorator;
//import com.nhaarman.listviewanimations.appearance.simple.AlphaInAnimationAdapter;
//import com.nhaarman.listviewanimations.util.StickyListHeadersListViewWrapper;

//import se.emilsjolander.stickylistheaders.StickyListHeadersListView;


using Android.App;
using Android.OS;
using com.refractored.components.stickylistheaders;
using Com.Nhaarman.ListviewAnimations.appearance;
using Com.Nhaarman.ListviewAnimations.Appearance.Simple;
using Com.Nhaarman.ListviewAnimations.Util;


namespace ListviewAnimations.Sample
{
    [Activity(Label = "StickyListHeadersActivity(.Net)") ]
     
    public class StickyListHeadersActivity : BaseActivity
    {

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_stickylistheaders);

            StickyListHeadersListView listView = (StickyListHeadersListView)FindViewById(Resource.Id.activity_stickylistheaders_listview);
            listView.SetFitsSystemWindows(true);

            MyListAdapter adapter = new MyListAdapter(this);
            AlphaInAnimationAdapter animationAdapter = new AlphaInAnimationAdapter(adapter);
            StickyListHeadersAdapterDecorator stickyListHeadersAdapterDecorator = new StickyListHeadersAdapterDecorator(animationAdapter);
            stickyListHeadersAdapterDecorator.setListViewWrapper(new StickyListHeadersListViewWrapper(listView));

            //assert animationAdapter.getViewAnimator() != null;
            animationAdapter.getViewAnimator().setInitialDelayMillis(500);

            //assert stickyListHeadersAdapterDecorator.getViewAnimator() != null;
            stickyListHeadersAdapterDecorator.getViewAnimator().setInitialDelayMillis(500);

            listView.Adapter=stickyListHeadersAdapterDecorator;
        }
    }
}