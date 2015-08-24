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

//import android.content.Context;
//import android.os.Bundle;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.AdapterView;
//import android.widget.TextView;
//import android.widget.Toast;

//import com.haarman.listviewanimations.MyListActivity;
//import com.haarman.listviewanimations.R;
//import com.nhaarman.listviewanimations.ArrayAdapter;
//import com.nhaarman.listviewanimations.itemmanipulation.DynamicListView;
//import com.nhaarman.listviewanimations.itemmanipulation.dragdrop.OnItemMovedListener;
//import com.nhaarman.listviewanimations.itemmanipulation.dragdrop.TouchViewDraggableManager;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.SimpleSwipeUndoAdapter;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.UndoAdapter;
//import com.nhaarman.listviewanimations.appearance.simple.AlphaInAnimationAdapter;

//import java.util.Arrays;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations;
using Com.Nhaarman.ListviewAnimations.Appearance.Simple;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo;
using Java.Lang;

namespace ListviewAnimations.Sample.itemmanipulation
{
    [Activity(Label = "DynamicListViewActivity(.Net)")]
    public class DynamicListViewActivity : MyListActivity
    {

        private static readonly int INITIAL_DELAY_MILLIS = 300;

        private int mNewItemCount;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_dynamiclistview);

            DynamicListView listView = (DynamicListView)FindViewById(Resource.Id.activity_dynamiclistview_listview);

            listView.AddHeaderView(LayoutInflater.From(this).Inflate(Resource.Layout.activity_dynamiclistview_header, listView, false));

            /* Setup the adapter */
            Com.Nhaarman.ListviewAnimations.ArrayAdapter<string> adapter = new MyListAdapter(this);
            SimpleSwipeUndoAdapter simpleSwipeUndoAdapter = new SimpleSwipeUndoAdapter(adapter, this, new MyOnDismissCallback(adapter, this));
            AlphaInAnimationAdapter animAdapter = new AlphaInAnimationAdapter(simpleSwipeUndoAdapter);
            animAdapter.setAbsListView(listView);
            //assert animAdapter.getViewAnimator() != null;
            animAdapter.getViewAnimator().setInitialDelayMillis(INITIAL_DELAY_MILLIS);
            listView.SetAdapter ( animAdapter);

            /* Enable drag and drop functionality */
            listView.enableDragAndDrop();
            listView.setDraggableManager(new TouchViewDraggableManager(Resource.Id.list_row_draganddrop_touchview));
            listView.setOnItemMovedListener(new MyOnItemMovedListener(adapter, this));

            listView.OnItemLongClickListener = new MyOnItemLongClickListener(listView);

            /* Enable swipe to dismiss */
            listView.enableSimpleSwipeUndo();

            /* Add new items on item click */
            listView.OnItemClickListener = new MyOnItemClickListener(listView, this);
        }

        private class MyListAdapter : Com.Nhaarman.ListviewAnimations.ArrayAdapter<string>, UndoAdapter
        {

            private Context mContext;

            public MyListAdapter(Context context)
            {
                mContext = context;
                for (int i = 0; i < 20; i++)
                {
                    add(mContext.GetString(Resource.String.row_number, i));
                }
            }

            //@Override
            public override long GetItemId(int position)
            {
                return GetItem2(position).GetHashCode();
                //return getItems()[position].GetHashCode();
            }

            public override bool HasStableIds
            {
                get
                {
                    return true;
                }
            }

            ////@Override
            //public bool hasStableIds() {
            //    return true;
            //}

            //@Override
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                if (view == null)
                {
                    view = LayoutInflater.From(mContext).Inflate(Resource.Layout.list_row_dynamiclistview, parent, false);
                }

                ((TextView)view.FindViewById(Resource.Id.list_row_draganddrop_textview)).Text = GetItem2(position);

                return view;
            }

            //@NonNull
            //@Override
            public View getUndoView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView;
                if (view == null)
                {
                    view = LayoutInflater.From(mContext).Inflate(Resource.Layout.undo_row, parent, false);
                }
                return view;
            }

            //@NonNull
            //@Override
            public View getUndoClickView(View view)
            {
                return view.FindViewById(Resource.Id.undo_row_undobutton);
            }
        }

        private class MyOnItemLongClickListener : Java.Lang.Object, AdapterView.IOnItemLongClickListener
        {

            private DynamicListView mListView;

            public MyOnItemLongClickListener(DynamicListView listView)
            {
                mListView = listView;
            }

            //@Override
            public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
            {
                if (mListView != null)
                {
                    mListView.startDragging(position - mListView.HeaderViewsCount);
                }
                return true;
            }
        }

        private class MyOnDismissCallback : OnDismissCallback
        {

            private Com.Nhaarman.ListviewAnimations.ArrayAdapter<string> mAdapter;

            //@Nullable
            private Toast mToast;
            private Context mContext;

            public MyOnDismissCallback(Com.Nhaarman.ListviewAnimations.ArrayAdapter<string> adapter, Context context)
            {
                mAdapter = adapter;
                mContext = context;
            }

            //@Override
            public void onDismiss(ViewGroup listView, int[] reverseSortedPositions)
            {
                foreach (int position in reverseSortedPositions)
                {
                    mAdapter.Remove(position);
                }

                if (mToast != null)
                {
                    mToast.Cancel();
                }
                mToast = Toast.MakeText(
                         mContext,
                    //mContext.GetString(Resource.String.removed_positions, Arrays.toString(reverseSortedPositions)),

                        mContext.GetString(Resource.String.removed_positions, reverseSortedPositions.ToString()),
                        ToastLength.Long
                );
                mToast.Show();
            }
        }

        private class MyOnItemMovedListener : Java.Lang.Object, OnItemMovedListener
        {

            private Com.Nhaarman.ListviewAnimations.ArrayAdapter<string> mAdapter;

            private Toast mToast;
            private Context mContext;

            public MyOnItemMovedListener(Com.Nhaarman.ListviewAnimations.ArrayAdapter<string> adapter, Context context)
            {
                mAdapter = adapter;
                mContext = context;
            }

            //@Override
            public void onItemMoved(int originalPosition, int newPosition)
            {
                if (mToast != null)
                {
                    mToast.Cancel();
                }

                mToast = Toast.MakeText(mContext, mContext.GetString(Resource.String.moved, mAdapter.GetItem(newPosition), newPosition), ToastLength.Short);
                mToast.Show();
            }
        }

        private class MyOnItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {

            private DynamicListView mListView;
            private DynamicListViewActivity minstance;

            public MyOnItemClickListener(DynamicListView listView, DynamicListViewActivity instance)
            {
                mListView = listView;
                minstance = instance;
            }

            //@Override
            public void OnItemClick(AdapterView parent, View view, int position, long id)
            {
                mListView.insert(position, minstance.GetString(Resource.String.newly_added_item, minstance.mNewItemCount));
                minstance.mNewItemCount++;
            }
        }
    }
}