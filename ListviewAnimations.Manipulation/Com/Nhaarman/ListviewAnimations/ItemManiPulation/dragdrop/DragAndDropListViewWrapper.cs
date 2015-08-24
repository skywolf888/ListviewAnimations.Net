using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
//package com.nhaarman.listviewanimations.itemmanipulation.dragdrop;

//import android.widget.AbsListView;

//import com.nhaarman.listviewanimations.util.ListViewWrapper;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{
    public interface DragAndDropListViewWrapper : ListViewWrapper
    {

        void setOnScrollListener(AbsListView.IOnScrollListener onScrollListener);

        int pointToPosition(int x, int y);

        int computeVerticalScrollOffset();

        int computeVerticalScrollExtent();

        int computeVerticalScrollRange();
    }
}