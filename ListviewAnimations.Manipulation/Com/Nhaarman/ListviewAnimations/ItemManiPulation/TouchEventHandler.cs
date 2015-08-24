//package com.nhaarman.listviewanimations.itemmanipulation;

//import android.support.annotation.NonNull;
//import android.view.MotionEvent;

using Android.Views;

namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation
{
    public interface TouchEventHandler
    {

        bool onTouchEvent(MotionEvent mevent);

        bool isInteracting();

    }
}