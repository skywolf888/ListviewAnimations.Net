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
//package com.nhaarman.listviewanimations.itemmanipulation.swipedismiss;

//import android.os.Handler;
//import android.support.annotation.NonNull;
//import android.view.View;
//import android.view.ViewGroup;

//import com.nhaarman.listviewanimations.util.AdapterViewUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorListenerAdapter;
//import com.nineoldandroids.animation.ValueAnimator;

//import java.util.Collection;
//import java.util.Collections;
//import java.util.LinkedList;
//import java.util.List;

/**
 * A {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.SwipeTouchListener} that directly dismisses the items when swiped.
 */

using Android.Animation;
using Android.OS;
using Android.Views;
using Com.Nhaarman.ListviewAnimations.Util;
using Java.Lang;
using System.Collections.Generic;

namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss
{
    public class SwipeDismissTouchListener : SwipeTouchListener
    {

        /**
         * The callback which gets notified of dismissed items.
         */
        //@NonNull
        private IOnDismissCallback mCallback;

        /**
         * The duration of the dismiss animation
         */
        private long mDismissAnimationTime;

        /**
         * The {@link android.view.View}s that have been dismissed.
         */
        //@NonNull
        private ICollection<View> mDismissedViews = new LinkedList<View>();

        /**
         * The dismissed positions.
         */
        //@NonNull
        private List<int> mDismissedPositions = new List<int>();

        /**
         * The number of active dismiss animations.
         */
        private int mActiveDismissCount;

        /**
         * A handler for posting {@link Runnable}s.
         */
        //@NonNull
        private Handler mHandler = new Handler();

        /**
         * Constructs a new {@code SwipeDismissTouchListener} for the given {@link android.widget.AbsListView}.
         *
         * @param listViewWrapper The {@code ListViewWrapper} containing the ListView whose items should be dismissable.
         * @param callback    The callback to trigger when the user has indicated that he
         */
        //@SuppressWarnings("UnnecessaryFullyQualifiedName")
        public SwipeDismissTouchListener(IListViewWrapper listViewWrapper, IOnDismissCallback callback)
            : base(listViewWrapper)
        {

            mCallback = callback;
            mDismissAnimationTime = listViewWrapper.getListView().Context.Resources.GetInteger(Android.Resource.Integer.ConfigShortAnimTime);
        }

        /**
         * Dismisses the {@link android.view.View} corresponding to given position.
         * Calling this method has the same effect as manually swiping an item off the screen.
         *
         * @param position the position of the item in the {@link android.widget.ListAdapter}. Must be visible.
         */
        public void dismiss(int position)
        {
            fling(position);
        }

        //@Override
        public override void fling(int position)
        {
            int firstVisiblePosition = getListViewWrapper().getFirstVisiblePosition();
            int lastVisiblePosition = getListViewWrapper().getLastVisiblePosition();

            if (firstVisiblePosition <= position && position <= lastVisiblePosition)
            {
                base.fling(position);
            }
            else if (position > lastVisiblePosition)
            {
                directDismiss(position);
            }
            else
            {
                dismissAbove(position);
            }
        }

        protected void directDismiss(int position)
        {
            mDismissedPositions.Add(position);
            finalizeDismiss();
        }

        private void dismissAbove(int position)
        {
            View view = AdapterViewUtil.getViewForPosition(getListViewWrapper(), getListViewWrapper().getFirstVisiblePosition());

            if (view != null)
            {
                view.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                int scrollDistance = view.MeasuredHeight;

                getListViewWrapper().smoothScrollBy(scrollDistance, (int)mDismissAnimationTime);
                mHandler.PostDelayed(new RestoreScrollRunnable(scrollDistance, position,this), mDismissAnimationTime);
            }
        }

        //@Override
        protected override void afterCancelSwipe(View view, int position)
        {
            finalizeDismiss();
        }

        //@Override
        protected override bool willLeaveDataSetOnFling(View view, int position)
        {
            return true;
        }

        //@Override
        protected override void afterViewFling(View view, int position)
        {
            performDismiss(view, position);
        }

        /**
         * Animates the dismissed list item to zero-height and fires the dismiss callback when all dismissed list item animations have completed.
         *
         * @param view the dismissed {@link android.view.View}.
         */
        protected virtual void performDismiss(View view, int position)
        {
            mDismissedViews.Add(view);
            mDismissedPositions.Add(position);

            ValueAnimator animator = ValueAnimator.OfInt(view.Height, 1);
            animator.SetDuration(mDismissAnimationTime);
            animator.AddUpdateListener(new DismissAnimatorUpdateListener(view));
            animator.AddListener(new DismissAnimatorListener(this));
            animator.Start();

            mActiveDismissCount++;
        }

        /**
         * If necessary, notifies the {@link OnDismissCallback} to remove dismissed object from the adapter,
         * and restores the {@link android.view.View} presentations.
         */
        protected virtual void finalizeDismiss()
        {
            if (mActiveDismissCount == 0 && getActiveSwipeCount() == 0)
            {
                restoreViewPresentations(mDismissedViews);
                notifyCallback(mDismissedPositions);

                mDismissedViews.Clear();
                mDismissedPositions.Clear();
            }
        }

        /**
         * Notifies the {@link OnDismissCallback} of dismissed items.
         *
         * @param dismissedPositions the positions that have been dismissed.
         */
        protected void notifyCallback(List<int> dismissedPositions)
        {
            if (dismissedPositions.Count != 0)
            {

                dismissedPositions.Reverse();
                dismissedPositions.Sort();
                //Collections.sort(dismissedPositions, Collections.reverseOrder());

                int[] dismissPositions = new int[dismissedPositions.Count];
                int i = 0;
                foreach (int dismissedPosition in dismissedPositions)
                {
                    dismissPositions[i] = dismissedPosition;
                    i++;
                }
                mCallback.onDismiss(getListViewWrapper().getListView(), dismissPositions);
            }
        }

        /**
         * Restores the presentation of given {@link android.view.View}s by calling {@link #restoreViewPresentation(android.view.View)}.
         */
        protected void restoreViewPresentations(IEnumerable<View> views)
        {
            foreach (View view in views)
            {
                restoreViewPresentation(view);
            }
        }

        //@Override
        protected override void restoreViewPresentation(View view)
        {
            base.restoreViewPresentation(view);
            ViewGroup.LayoutParams layoutParams = view.LayoutParameters;
            layoutParams.Height = 0;
            view.LayoutParameters = layoutParams;
        }

        protected int getActiveDismissCount()
        {
            return mActiveDismissCount;
        }

        public long getDismissAnimationTime()
        {
            return mDismissAnimationTime;
        }

        /**
         * An {@link com.nineoldandroids.animation.ValueAnimator.AnimatorUpdateListener} which applies height animation to given {@link android.view.View}.
         */
        private class DismissAnimatorUpdateListener : Java.Lang.Object,ValueAnimator.IAnimatorUpdateListener
        {

            //@NonNull
            private View mView;

            public DismissAnimatorUpdateListener(View view)
            {
                mView = view;
            }

            //@Override
            public   void OnAnimationUpdate(ValueAnimator animation)
            {
                ViewGroup.LayoutParams layoutParams = mView.LayoutParameters;
                layoutParams.Height = (int)animation.AnimatedValue;
                mView.LayoutParameters = layoutParams;
            }
        }

        private class DismissAnimatorListener : AnimatorListenerAdapter
        {
            private SwipeDismissTouchListener minst;


            public DismissAnimatorListener(SwipeDismissTouchListener instance)
            {
                minst = instance;
            }
            //@Override
            public  override void OnAnimationEnd(Animator animation)
            {
                minst.mActiveDismissCount--;
                minst.finalizeDismiss();
            }
        }

        /**
         * A {@link Runnable} which applies the dismiss of a position, and restores the scroll position.
         */
        private class RestoreScrollRunnable : Java.Lang.Object,IRunnable
        {

            private int mScrollDistance;
            private int mPosition;
            private SwipeDismissTouchListener minst;

            /**
             * Creates a new {@code RestoreScrollRunnable}.
             *
             * @param scrollDistance The scroll distance in pixels to restore.
             * @param position       the position to dismiss
             */
            public RestoreScrollRunnable(int scrollDistance, int position, SwipeDismissTouchListener instance)
            {
                mScrollDistance = scrollDistance;
                mPosition = position;
                minst = instance;
            }

            //@Override
            public void Run()
            {
                minst.getListViewWrapper().smoothScrollBy(-mScrollDistance, 1);
                minst.directDismiss(mPosition);
            }
        }
    }
}