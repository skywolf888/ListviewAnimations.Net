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

//package com.nhaarman.listviewanimations.util;

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.widget.ListAdapter;

using Android.Views;
using Android.Widget;
using com.refractored.components.stickylistheaders;
//import se.emilsjolander.stickylistheaders.StickyListHeadersListView;
 


namespace Com.Nhaarman.ListviewAnimations.Util
{
    public class StickyListHeadersListViewWrapper : IListViewWrapper
    {

        //@NonNull
        private StickyListHeadersListView mListView;

        public StickyListHeadersListViewWrapper(StickyListHeadersListView listView)
        {
            mListView = listView;
        }

        //@NonNull
        //@Override
        public ViewGroup getListView()
        {
            return mListView;
        }

        //@Nullable
        //@Override
        public View getChildAt(int index)
        {
            return mListView.GetChildAt(index);
            //return mListView.getListChildAt(index);
        }

        //@Override
        public int getFirstVisiblePosition()
        {
            return mListView.FirstVisiblePosition;
        }

        //@Override
        public int getLastVisiblePosition()
        {
            return mListView.LastVisiblePosition;
        }

        //@Override
        public int getCount()
        {
            return mListView.Count;
        }

        //@Override
        public int getChildCount()
        {
            return mListView.ChildCount;
        }

        //@Override
        public int getHeaderViewsCount()
        {
            return mListView.HeaderViewsCount;
        }

        //@Override
        public int getPositionForView(View view)
        {
            return mListView.GetPositionForView(view);
        }

        //@NonNull
        //@Override
        public IListAdapter getAdapter()
        {
            return mListView.Adapter;
        }

        //@Override
        public void smoothScrollBy(int distance, int duration)
        {
            mListView.SmoothScrollBy(distance, duration);
        }
    }
}