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

using Android.Views;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.widget.AbsListView;
//import android.widget.ListAdapter;
//import android.widget.ListView;
using Android.Widget;
namespace Com.Nhaarman.ListviewAnimations.Util
{
public class AbsListViewWrapper : ListViewWrapper {

    //@NonNull
    private AbsListView mAbsListView;

    public AbsListViewWrapper(AbsListView absListView) {
        mAbsListView = absListView;
    }

    //@Override
    //@NonNull
    public ViewGroup getListView() {
        return mAbsListView;
    }

    //@Nullable
    //@Override
    public View getChildAt(int index)
    {
        return mAbsListView.GetChildAt(index);
    }

    //@Override
    public int getFirstVisiblePosition() {
        return mAbsListView.FirstVisiblePosition;
    }


    //@Override
    public int getLastVisiblePosition() {
        return mAbsListView.LastVisiblePosition;
    }

    //@Override
    public int getCount() {
        return mAbsListView.Count;
    }

    //@Override
    public int getChildCount() {
        return mAbsListView.ChildCount;
    }

    //@Override
    public int getHeaderViewsCount() {
        int result = 0;
        if (mAbsListView is ListView) {
            result = ((ListView) mAbsListView).HeaderViewsCount;
        }
        return result;
    }

    //@Override
    public int getPositionForView(View view) {
        return mAbsListView.GetPositionForView(view);
    }

    //@Override
    public IListAdapter getAdapter() {
        return mAbsListView.Adapter;
    }

    //@Override
    public void smoothScrollBy(int distance,  int duration) {
        mAbsListView.SmoothScrollBy(distance, duration);
    }

}
}