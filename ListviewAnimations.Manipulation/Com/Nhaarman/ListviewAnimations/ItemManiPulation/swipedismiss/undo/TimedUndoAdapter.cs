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
//package com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo;

//import android.content.Context;
//import android.os.Handler;
//import android.support.annotation.NonNull;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback;

//import java.util.HashMap;
//import java.util.Map;

using Android.Content;
/**
 * A {@link SimpleSwipeUndoAdapter} which automatically dismisses items after a timeout.
 */
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{
    public class TimedUndoAdapter : SimpleSwipeUndoAdapter
    {

        /**
         * The default time in milliseconds before an item in the undo state should automatically dismiss.
         */
        public static readonly long DEFAULT_TIMEOUT_MS = 3000;

        /**
         * The time in milliseconds before an item in the undo state should automatically dismiss.
         * Defaults to {@value #DEFAULT_TIMEOUT_MS}.
         */
        private long mTimeoutMs = DEFAULT_TIMEOUT_MS;

        /**
         * A {@link android.os.Handler} to post {@link TimeoutRunnable}s to.
         */
        //@NonNull
        private Handler mHandler = new Handler();

        /**
         * The {@link TimeoutRunnable}s which are posted. Keys are positions.
         */
        //noinspection UseSparseArrays
        private Dictionary<int, TimeoutRunnable> mRunnables = new Dictionary<int, TimeoutRunnable>();

        /**
         * Creates a new {@code TimedUndoAdapterGen}, decorating given {@link android.widget.BaseAdapter}.
         *
         * @param undoAdapter     the {@link android.widget.BaseAdapter} that is decorated. Must implement
         *                        {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.UndoAdapter}.
         * @param context         the {@link android.content.Context}.
         * @param dismissCallback the {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback} that is notified of dismissed items.
         */
        //public <V extends BaseAdapter & UndoAdapter> TimedUndoAdapter(@NonNull  V undoAdapter, @NonNull  Context context, @NonNull  OnDismissCallback
        //        dismissCallback) {
        //    super(undoAdapter, context, dismissCallback);
        //}

        public  TimedUndoAdapter(BaseAdapter undoAdapter,    Context context,    OnDismissCallback
                dismissCallback)
            :base(undoAdapter, context, dismissCallback)
        {
            //super(undoAdapter, context, dismissCallback);
        }

        /**
         * Sets the time in milliseconds after which an item in the undo state should automatically dismiss.
         * Defaults to {@value #DEFAULT_TIMEOUT_MS}.
         */
        public void setTimeoutMs(long timeoutMs)
        {
            mTimeoutMs = timeoutMs;
        }

        //@Override
        public override void onUndoShown(View view, int position)
        {
            base.onUndoShown(view, position);
            TimeoutRunnable timeoutRunnable = new TimeoutRunnable(position,this);
            mRunnables.Add(position, timeoutRunnable);
            mHandler.PostDelayed(timeoutRunnable, mTimeoutMs);
        }

        //@Override
        public  override void onUndo(View view, int position)
        {
            base.onUndo(view, position);
            cancelCallback(position);
        }

        //@Override
        public override void onDismiss(View view, int position)
        {
            base.onDismiss(view, position);
            cancelCallback(position);
        }

        //@Override
        public override void dismiss(int position)
        {
            base.dismiss(position);
            cancelCallback(position);
        }

        private void cancelCallback(int position)
        {
            IRunnable timeoutRunnable = mRunnables[position];
            if (timeoutRunnable != null)
            {
                mHandler.RemoveCallbacks(timeoutRunnable);
                mRunnables.Remove(position);
            }
        }

        //@Override
        public void onDismiss(ViewGroup listView, int[] reverseSortedPositions)
        {
            base.onDismiss(listView, reverseSortedPositions);

            /* Adjust the pending timeout positions accordingly wrt the given dismissed positions */
            //noinspection UseSparseArrays
            Dictionary<int, TimeoutRunnable> newRunnables = new Dictionary<int, TimeoutRunnable>();
            foreach (int position in reverseSortedPositions)
            {
                foreach (int key in mRunnables.Keys)
                {
                    TimeoutRunnable runnable = mRunnables[key];
                    if (key > position)
                    {
                        int ntemp = key;
                        ntemp--;
                        runnable.setPosition(ntemp);
                        newRunnables.Add(ntemp, runnable);
                    }
                    else if (key != position)
                    {
                        newRunnables.Add(key, runnable);
                    }
                }

                mRunnables.Clear();
                 
                //mRunnables.putAll(newRunnables);
                mRunnables = newRunnables.ToDictionary(k => k.Key, v => v.Value);
                newRunnables.Clear();
            }
        }

        /**
         * A {@link Runnable} class which dismisses a position when executed.
         */
        private class TimeoutRunnable : Java.Lang.Object,IRunnable
        {

            private int mPosition;
            private TimedUndoAdapter minst;

            public TimeoutRunnable(int position,TimedUndoAdapter instance)
            {
                mPosition = position;
                minst = instance;
            }

            //@Override
            public void Run()
            {
                minst.dismiss(mPosition);
            }

            public int getPosition()
            {
                return mPosition;
            }

            public void setPosition(int position)
            {
                mPosition = position;
            }
        }
    }
}