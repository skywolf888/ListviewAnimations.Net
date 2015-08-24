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

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.BaseAdapterDecorator;
//import com.nhaarman.listviewanimations.itemmanipulation.DynamicListView;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.DismissableManager;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;

using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;

namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{

    /**
     * Adds swipe-undo behaviour to the {@link android.widget.AbsListView}, using a {@link SwipeUndoTouchListener}.
     */
    public abstract class SwipeUndoAdapter : BaseAdapterDecorator
    {

        /**
         * The {@link SwipeUndoTouchListener} that is set to the {@link android.widget.AbsListView}.
         */
        //@Nullable
        private SwipeUndoTouchListener mSwipeUndoTouchListener;

        /**
         * The {@link UndoCallback} that is used.
         */
        //@NonNull
        private IUndoCallback mUndoCallback;

        /**
         * Create a new {@code SwipeUndoAdapter}, decorating given {@link android.widget.BaseAdapter}.
         *
         * @param baseAdapter  the {@link android.widget.BaseAdapter} to decorate.
         * @param undoCallback the {@link UndoCallback} that is used.
         */
        protected SwipeUndoAdapter(BaseAdapter baseAdapter, IUndoCallback undoCallback)
            : base(baseAdapter)
        {

            mUndoCallback = undoCallback;
        }

        //@Override
        public override void setListViewWrapper(IListViewWrapper listViewWrapper)
        {

            base.setListViewWrapper(listViewWrapper);
            mSwipeUndoTouchListener = new SwipeUndoTouchListener(listViewWrapper, mUndoCallback);

            if (!(listViewWrapper.getListView() is DynamicListView))
            {
                listViewWrapper.getListView().SetOnTouchListener(mSwipeUndoTouchListener);
            }
        }

        /**
         * Sets the {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.DismissableManager} to specify which views can or cannot be swiped.
         *
         * @param dismissableManager {@code null} for no restrictions.
         */
        public void setDismissableManager(IDismissableManager dismissableManager)
        {
            if (mSwipeUndoTouchListener == null)
            {
                throw new Java.Lang.IllegalStateException("You must call setAbsListView() first.");
            }
            mSwipeUndoTouchListener.setDismissableManager(dismissableManager);
        }

        public void setSwipeUndoTouchListener(SwipeUndoTouchListener swipeUndoTouchListener)
        {
            mSwipeUndoTouchListener = swipeUndoTouchListener;
        }

        //@NonNull
        //@Override
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (getListViewWrapper() == null)
            {
                throw new Java.Lang.IllegalArgumentException("Call setAbsListView() on this SwipeUndoAdapter before setAdapter()!");
            }
            return base.GetView(position, convertView, parent);
        }

        /**
         * Sets the {@link UndoCallback} to use.
         */
        public void setUndoCallback(IUndoCallback undoCallback)
        {
            mUndoCallback = undoCallback;
        }

        //@NonNull
        public IUndoCallback getUndoCallback()
        {
            return mUndoCallback;
        }

        /**
         * Performs the undo animation and restores the original state for given {@link android.view.View}.
         *
         * @param view the parent {@code View} which contains both primary and undo {@code View}s.
         */
        public   void undo(View view)
        {
            mSwipeUndoTouchListener.undo(view);
        }

        /**
         * Dismisses the {@link android.view.View} corresponding to given position. Calling this method has the same effect as manually swiping an item off the screen.
         *
         * @param position the position of the item in the {@link android.widget.ListAdapter}. Must be visible.
         */
        public virtual void dismiss(int position)
        {
            mSwipeUndoTouchListener.dismiss(position);
        }
    }
}