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
//package com.haarman.listviewanimations.itemmanipulation.expandablelistitems;

//import android.os.Bundle;
//import android.view.Menu;
//import android.view.MenuItem;
//import android.widget.Toast;

//import com.haarman.listviewanimations.MyListActivity;
//import com.haarman.listviewanimations.R;
//import com.nhaarman.listviewanimations.appearance.simple.AlphaInAnimationAdapter;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Appearance.Simple;


namespace ListviewAnimations.Sample.itemmanipulation.expandablelistitems
{
     [Activity(Label = "ExpandableListItemActivity(.Net)")]
    public class ExpandableListItemActivity : MyListActivity
    {

        private static readonly int INITIAL_DELAY_MILLIS = 500;
        private MyExpandableListItemAdapter mExpandableListItemAdapter;

        private bool mLimited;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mExpandableListItemAdapter = new MyExpandableListItemAdapter(this);
            AlphaInAnimationAdapter alphaInAnimationAdapter = new AlphaInAnimationAdapter(mExpandableListItemAdapter);
            alphaInAnimationAdapter.setAbsListView(getListView());

            //assert alphaInAnimationAdapter.getViewAnimator() != null;
            alphaInAnimationAdapter.getViewAnimator().setInitialDelayMillis(INITIAL_DELAY_MILLIS);

            getListView().Adapter = alphaInAnimationAdapter;

            Toast.MakeText(this, Resource.String.explainexpand, ToastLength.Long).Show();
        }

        //@Override
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_expandablelistitem, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        //@Override
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_expandable_limit:
                    mLimited = !mLimited;
                    item.SetChecked(mLimited);
                    mExpandableListItemAdapter.setLimit(mLimited ? 2 : 0);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}