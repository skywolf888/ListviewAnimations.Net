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

//package com.nhaarman.listviewanimations.appearance;

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.BaseAdapterDecorator;
//import com.nhaarman.listviewanimations.util.AnimatorUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nhaarman.listviewanimations.util.StickyListHeadersListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.ObjectAnimator;

using Android.Animation;
using Android.Views;
using Android.Widget;
using com.refractored.components.stickylistheaders;
//import se.emilsjolander.stickylistheaders.StickyListHeadersAdapter;
//import se.emilsjolander.stickylistheaders.StickyListHeadersListView;
using Com.Nhaarman.ListviewAnimations.Appearance;
using Com.Nhaarman.ListviewAnimations.Util;

namespace Com.Nhaarman.ListviewAnimations.appearance
{
    /**
     * A {@link com.nhaarman.listviewanimations.BaseAdapterDecorator} which can be used to animate header views provided by a
     * {@link se.emilsjolander.stickylistheaders.StickyListHeadersAdapter}.
     */
    public class StickyListHeadersAdapterDecorator : BaseAdapterDecorator, IStickyListHeadersAdapter
    {

        /**
         * Alpha property.
         */
        private static readonly string ALPHA = "alpha";

        /**
         * The decorated {@link se.emilsjolander.stickylistheaders.StickyListHeadersAdapter}.
         */
        //@NonNull
        private IStickyListHeadersAdapter mStickyListHeadersAdapter;

        /**
         * The {@link com.nhaarman.listviewanimations.appearance.ViewAnimator} responsible for animating the Views.
         */
        //@Nullable
        private Com.Nhaarman.ListviewAnimations.Appearance.ViewAnimator mViewAnimator;

        /**
         * Create a new {@code StickyListHeadersAdapterDecorator}, decorating given {@link android.widget.BaseAdapter}.
         *
         * @param baseAdapter the {@code BaseAdapter} to decorate. If this is a {@code BaseAdapterDecorator}, it should wrap an instance of
         *                    {@link se.emilsjolander.stickylistheaders.StickyListHeadersAdapter}.
         */
        public StickyListHeadersAdapterDecorator(BaseAdapter baseAdapter)
            : base(baseAdapter)
        {
            //super(baseAdapter);

            BaseAdapter adapter = baseAdapter;
            while (adapter is BaseAdapterDecorator)
            {
                adapter = ((BaseAdapterDecorator)adapter).getDecoratedBaseAdapter();
            }

            if (!(adapter is IStickyListHeadersAdapter))
            {
                //.getCanonicalName()
                throw new Java.Lang.IllegalArgumentException(adapter.GetType().FullName + " does not implement StickyListHeadersAdapter");
            }

            mStickyListHeadersAdapter = (IStickyListHeadersAdapter)adapter;
        }

        /**
         * Sets the {@link se.emilsjolander.stickylistheaders.StickyListHeadersListView} that this adapter will be bound to.
         */
        public void setStickyListHeadersListView(StickyListHeadersListView listView)
        {
            IListViewWrapper stickyListHeadersListViewWrapper = new StickyListHeadersListViewWrapper(listView);
            setListViewWrapper(stickyListHeadersListViewWrapper);
        }

        /**
         * Returns the {@link com.nhaarman.listviewanimations.appearance.ViewAnimator} responsible for animating the header Views in this adapter.
         */
        //@Nullable
        public Com.Nhaarman.ListviewAnimations.Appearance.ViewAnimator getViewAnimator()
        {
            return mViewAnimator;
        }

        //@Override
        public override void setListViewWrapper(IListViewWrapper listViewWrapper)
        {
            base.setListViewWrapper(listViewWrapper);
            mViewAnimator = new Com.Nhaarman.ListviewAnimations.Appearance.ViewAnimator(listViewWrapper);
        }

        //@Override
        public View GetHeaderView(int position, View convertView, ViewGroup parent)
        {
            if (getListViewWrapper() == null)
            {
                throw new Java.Lang.IllegalStateException("Call setStickyListHeadersListView() on this AnimationAdapter first!");
            }

            if (convertView != null)
            {
                //assert mViewAnimator != null;
                mViewAnimator.cancelExistingAnimation(convertView);
            }

            View itemView = mStickyListHeadersAdapter.GetHeaderView(position, convertView, parent);

            animateViewIfNecessary(position, itemView, parent);
            return itemView;
        }

        /**
         * Animates given View if necessary.
         *
         * @param position the position of the item the View represents.
         * @param view     the View that should be animated.
         * @param parent   the parent the View is hosted in.
         */
        private void animateViewIfNecessary(int position, View view, ViewGroup parent)
        {
            Animator[] childAnimators;
            if (getDecoratedBaseAdapter() is AnimationAdapter)
            {
                childAnimators = ((AnimationAdapter)getDecoratedBaseAdapter()).getAnimators(parent, view);
            }
            else
            {
                childAnimators = new Animator[0];
            }
            Animator alphaAnimator = ObjectAnimator.OfFloat(view, ALPHA, 0, 1);

            //assert mViewAnimator != null;
            mViewAnimator.animateViewIfNecessary(position, view, AnimatorUtil.concatAnimators(childAnimators, new Animator[0], alphaAnimator));
        }

        // @Override
        public long GetHeaderId(int position)
        {
            return mStickyListHeadersAdapter.GetHeaderId(position);
        }
    }
}