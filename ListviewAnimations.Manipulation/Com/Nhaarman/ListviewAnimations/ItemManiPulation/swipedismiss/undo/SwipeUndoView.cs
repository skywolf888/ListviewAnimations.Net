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
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.widget.FrameLayout;


using Android.Content;
using Android.Views;
using Android.Widget;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{

    /**
     * A convenience class which holds a primary and a undo {@link android.view.View}.
     */
    class SwipeUndoView : FrameLayout
    {

        /**
         * The primary {@link android.view.View}.
         */
        //@Nullable
        private View mPrimaryView;

        /**
         * The undo {@link android.view.View}.
         */
        //@Nullable
        private View mUndoView;

        /**
         * Creates a new {@code SwipeUndoView}.
         */
        internal SwipeUndoView(Context context)
            : base(context)
        {
            //super(context);
        }

        /**
         * Sets the primary {@link android.view.View}. Removes any existing primary {@code View} if present.
         */
         internal void setPrimaryView(View primaryView)
        {
            if (mPrimaryView != null)
            {
                RemoveView(mPrimaryView);
            }
            mPrimaryView = primaryView;
            AddView(mPrimaryView);
        }

        /**
         * Sets the undo {@link android.view.View}. Removes any existing primary {@code View} if present, and sets the visibility of the {@code undoView} to {@link #GONE}.
         */
        internal void setUndoView(View undoView)
        {
            if (mUndoView != null)
            {
                RemoveView(mUndoView);
            }
            mUndoView = undoView;
            mUndoView.Visibility=ViewStates.Gone;
            AddView(mUndoView);
        }

        /**
         * Returns the primary {@link android.view.View}.
         */
        //@Nullable
        internal View getPrimaryView()
        {
            return mPrimaryView;
        }

        /**
         * Returns the undo {@link android.view.View}.
         */
        //@Nullable
        internal View getUndoView()
        {
            return mUndoView;
        }
    }
}