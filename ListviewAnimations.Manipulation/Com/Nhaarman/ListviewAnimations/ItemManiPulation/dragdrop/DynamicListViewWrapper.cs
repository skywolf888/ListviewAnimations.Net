//package com.nhaarman.listviewanimations.itemmanipulation.dragdrop;

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.widget.AbsListView;
//import android.widget.ListAdapter;

//import com.nhaarman.listviewanimations.itemmanipulation.DynamicListView;
using Android.Views;
using Android.Widget;
using Java.Lang;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{
    public class DynamicListViewWrapper : DragAndDropListViewWrapper
    {

        ////@NonNull
        private DynamicListView mDynamicListView;

        public DynamicListViewWrapper(DynamicListView dynamicListView)
        {
            mDynamicListView = dynamicListView;
        }

        //@NonNull
        //@Override
        public ViewGroup getListView()
        {
            return mDynamicListView;
        }

        //@Nullable
        //@Override
        public View getChildAt(int index)
        {
            return mDynamicListView.GetChildAt(index);
        }

        //@Override
        public int getFirstVisiblePosition()
        {
            return mDynamicListView.FirstVisiblePosition;
        }

        //@Override
        public int getLastVisiblePosition()
        {
            return mDynamicListView.LastVisiblePosition;
        }

        //@Override
        public int getCount()
        {
            return mDynamicListView.Count;
        }

        //@Override
        public int getChildCount()
        {
            return mDynamicListView.ChildCount;
        }

        //@Override
        public int getHeaderViewsCount()
        {
            return mDynamicListView.HeaderViewsCount;
        }

        //@Override
        public int getPositionForView(View view)
        {
            return mDynamicListView.GetPositionForView(view);
        }

        //@Nullable
        //@Override
        public IListAdapter getAdapter()
        {
            return mDynamicListView.Adapter;
        }

        //@Override
        public void smoothScrollBy(int distance, int duration)
        {
            mDynamicListView.SmoothScrollBy(distance, duration);
        }

        //@Override
        public void setOnScrollListener(AbsListView.IOnScrollListener onScrollListener)
        {
            mDynamicListView.SetOnScrollListener(onScrollListener);
        }

        //@Override
        public int pointToPosition(int x, int y)
        {
            return mDynamicListView.PointToPosition(x, y);
        }

        //@Override
        public int computeVerticalScrollOffset()
        {
            throw new Exception();
            //return mDynamicListView.ComputeVerticalScrollOffset();
        }

        //@Override
        public int computeVerticalScrollExtent()
        {
            throw new Exception();
            //return mDynamicListView.computeVerticalScrollExtent();
        }

        //@Override
        public int computeVerticalScrollRange()
        {
            throw new Exception();
            //return mDynamicListView.computeVerticalScrollRange();
        }
    }
}