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
//package com.haarman.listviewanimations.appearance;

//import android.app.ActionBar;
//import android.content.Context;
//import android.os.Bundle;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;
//import android.widget.SpinnerAdapter;
//import android.widget.TextView;

//import com.haarman.listviewanimations.MyListActivity;
////import com.haarman.listviewanimations.MyListAdapter;
//import com.haarman.listviewanimations.R;
//import com.nhaarman.listviewanimations.ArrayAdapter;
//import com.nhaarman.listviewanimations.appearance.AnimationAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.AlphaInAnimationAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.ScaleInAnimationAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.SwingBottomInAnimationAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.SwingLeftInAnimationAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.SwingRightInAnimationAdapter;

//import java.util.Arrays;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Appearance;
using Com.Nhaarman.ListviewAnimations.Appearance.Simple;
namespace ListviewAnimations.Sample.appearance
{



     [Activity(Label = "DynamicListViewActivity(.Net)")]
    public class AppearanceExamplesActivity : MyListActivity, Android.App.ActionBar.IOnNavigationListener
    {

        private static readonly string SAVEDINSTANCESTATE_ANIMATIONADAPTER = "savedinstancestate_animationadapter";

        private BaseAdapter mAdapter;

        private AnimationAdapter mAnimAdapter;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mAdapter = new MyListAdapter(this);
            setAlphaAdapter();

            //assert getActionBar() != null;
            ActionBar.NavigationMode = ActionBarNavigationMode.List;
            //getActionBar().setNavigationMode(ActionBar.NAVIGATION_MODE_LIST);
            ActionBar.SetDisplayShowTitleEnabled(false);



            ISpinnerAdapter animSelectionAdapter = new AnimSelectionAdapter(this);//new AnimSelectionAdapter(this.BaseContext);
            ActionBar.SetListNavigationCallbacks(animSelectionAdapter, this);


        }

        private void setAlphaAdapter()
        {
            if (!(mAnimAdapter is AlphaInAnimationAdapter))
            {
                mAnimAdapter = new AlphaInAnimationAdapter(mAdapter);
                mAnimAdapter.setAbsListView(getListView());
                getListView().Adapter = mAnimAdapter;
            }
        }

        private void setLeftAdapter()
        {
            if (!(mAnimAdapter is SwingLeftInAnimationAdapter))
            {
                mAnimAdapter = new SwingLeftInAnimationAdapter(mAdapter);
                mAnimAdapter.setAbsListView(getListView());
                getListView().Adapter = mAnimAdapter;
            }
        }

        private void setRightAdapter()
        {
            if (!(mAnimAdapter is SwingRightInAnimationAdapter))
            {
                mAnimAdapter = new SwingRightInAnimationAdapter(mAdapter);
                mAnimAdapter.setAbsListView(getListView());
                getListView().Adapter = mAnimAdapter;
            }
        }

        private void setBottomAdapter()
        {
            if (!(mAnimAdapter is SwingBottomInAnimationAdapter))
            {
                mAnimAdapter = new SwingBottomInAnimationAdapter(mAdapter);
                mAnimAdapter.setAbsListView(getListView());
                getListView().Adapter = mAnimAdapter;
            }
        }

        private void setBottomRightAdapter()
        {
            mAnimAdapter = new SwingBottomInAnimationAdapter(new SwingRightInAnimationAdapter(mAdapter));
            mAnimAdapter.setAbsListView(getListView());
            getListView().Adapter = mAnimAdapter;
        }

        private void setScaleAdapter()
        {
            if (!(mAnimAdapter is ScaleInAnimationAdapter))
            {
                mAnimAdapter = new ScaleInAnimationAdapter(mAdapter);
                mAnimAdapter.setAbsListView(getListView());
                getListView().Adapter = mAnimAdapter;
            }
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutParcelable(SAVEDINSTANCESTATE_ANIMATIONADAPTER, mAnimAdapter.onSaveInstanceState());
            base.OnSaveInstanceState(outState);
        }


        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);

            mAnimAdapter.onRestoreInstanceState((IParcelable)savedInstanceState.GetParcelable(SAVEDINSTANCESTATE_ANIMATIONADAPTER));
        }

        //@Override
        public bool OnNavigationItemSelected(int itemPosition, long itemId)
        {
            switch (itemPosition)
            {
                case 0:
                    setAlphaAdapter();
                    return true;
                case 1:
                    setLeftAdapter();
                    return true;
                case 2:
                    setRightAdapter();
                    return true;
                case 3:
                    setBottomAdapter();
                    return true;
                case 4:
                    setBottomRightAdapter();
                    return true;
                case 5:
                    setScaleAdapter();
                    return true;
                default:
                    return false;
            }
        }

        private class AnimSelectionAdapter : Com.Nhaarman.ListviewAnimations.ArrayAdapter<string>
        {

            private Context mContext;


            public AnimSelectionAdapter(AppearanceExamplesActivity context)
            {
                mContext = context;
                string[] items = context.Resources.GetStringArray(Resource.Array.appearance_examples);

                //addAll(Arrays.asList(items));
                addAll(items);
            }

            //@Override
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                TextView tv = (TextView)convertView;
                if (tv == null)
                {
                    tv = (TextView)LayoutInflater.From(mContext).Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
                    tv.SetTextColor(mContext.Resources.GetColor(Android.Resource.Color.White));
                }

                tv.Text = GetItem2(position);

                return tv;
            }
        }
    }
}