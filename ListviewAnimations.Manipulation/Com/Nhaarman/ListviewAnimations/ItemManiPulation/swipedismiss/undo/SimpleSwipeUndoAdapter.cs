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
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.BaseAdapterDecorator;
//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback;

//import java.util.ArrayList;
//import java.util.Collection;


using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{

    /**
     * An implementation of {@link SwipeUndoAdapter} which puts the primary and undo {@link android.view.View} in a {@link android.widget.FrameLayout},
     * and handles the undo click event.
     */
    public class SimpleSwipeUndoAdapter : SwipeUndoAdapter, UndoCallback
    {

        //@NonNull
        private Context mContext;

        /**
         * The {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback} that is notified of dismissed items.
         */
        //@NonNull
        private OnDismissCallback mOnDismissCallback;

        /**
         * The {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.UndoAdapter} that provides the undo {@link android.view.View}s.
         */
        //@NonNull
        private UndoAdapter mUndoAdapter;

        /**
         * The positions of the items currently in the undo state.
         */
        private ICollection<int> mUndoPositions = new List<int>();

        /**
         * Create a new {@code SimpleSwipeUndoAdapterGen}, decorating given {@link android.widget.BaseAdapter}.
         *
         * @param undoAdapter     the {@link android.widget.BaseAdapter} that is decorated. Must implement
         *                        {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo.UndoAdapter}.
         * @param context         the {@link android.content.Context}.
         * @param dismissCallback the {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.OnDismissCallback} that is notified of dismissed items.
         */
        public SimpleSwipeUndoAdapter(BaseAdapter adapter, Context context,
                                            OnDismissCallback dismissCallback)

            : base(adapter, null)
        {
            // We fix this right away
            // noinspection ConstantConditions

            setUndoCallback(this);

            BaseAdapter undoAdapter = adapter;
            while (undoAdapter is BaseAdapterDecorator)
            {
                undoAdapter = ((BaseAdapterDecorator)undoAdapter).getDecoratedBaseAdapter();
            }

            if (!(undoAdapter is UndoAdapter))
            {
                throw new Java.Lang.IllegalStateException("BaseAdapter must implement UndoAdapter!");
            }

            mUndoAdapter = (UndoAdapter)undoAdapter;
            mContext = context;
            mOnDismissCallback = dismissCallback;
        }

        //@NonNull
        //@Override
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SwipeUndoView view = (SwipeUndoView)convertView;
            if (view == null)
            {
                view = new SwipeUndoView(mContext);
            }
            View primaryView = base.GetView(position, view.getPrimaryView(), view);
            view.setPrimaryView(primaryView);

            View undoView = mUndoAdapter.getUndoView(position, view.getUndoView(), view);
            view.setUndoView(undoView);

            mUndoAdapter.getUndoClickView(undoView).SetOnClickListener(new UndoClickListener(view, position,this));

            bool isInUndoState = mUndoPositions.Contains(position);
            primaryView.Visibility = isInUndoState ? ViewStates.Gone : ViewStates.Visible;
            undoView.Visibility = isInUndoState ? ViewStates.Visible : ViewStates.Gone;

            return view;
        }

        //@Override
        //@NonNull
        public   View getPrimaryView(View view)
        {
            View primaryView = ((SwipeUndoView)view).getPrimaryView();
            if (primaryView == null)
            {
                throw new Java.Lang.IllegalStateException("primaryView == null");
            }
            return primaryView;
            
        }

        //@Override
        //@NonNull
        public   View getUndoView(View view)
        {
            View undoView = ((SwipeUndoView)view).getUndoView();
            if (undoView == null)
            {
                throw new Java.Lang.IllegalStateException("undoView == null");
            }
            return undoView;
        }

        //@Override
        public virtual  void onUndoShown(View view, int position)
        {
            mUndoPositions.Add(position);
        }

        //@Override
        public virtual void onUndo(View view, int position)
        {
            mUndoPositions.Remove(position);
        }

        //@Override
        public virtual void onDismiss(View view, int position)
        {
            mUndoPositions.Remove(position);
        }

        //@Override
        public void onDismiss(ViewGroup listView, int[] reverseSortedPositions)
        {
            mOnDismissCallback.onDismiss(listView, reverseSortedPositions);

            ICollection<int> newUndoPositions = Util.processDeletions(mUndoPositions, reverseSortedPositions);
            mUndoPositions.Clear();
            //mUndoPositions.addAll(newUndoPositions);
            (mUndoPositions as List<int>).AddRange(newUndoPositions);
        }


        private class UndoClickListener : Java.Lang.Object,View.IOnClickListener
        {

            //@NonNull
            private SwipeUndoView mView;

            private int mPosition;
            private SimpleSwipeUndoAdapter minst;

            public UndoClickListener(SwipeUndoView view, int position,SimpleSwipeUndoAdapter instance)
            {
                mView = view;
                mPosition = position;
                minst = instance;
            }

            //@Override
            public void OnClick(View v)
            {
                minst.undo(mView);
            }
        }
    }
}