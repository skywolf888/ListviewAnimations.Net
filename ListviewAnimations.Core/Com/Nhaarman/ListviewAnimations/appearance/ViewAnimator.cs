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

//import android.annotation.SuppressLint;
//import android.os.Build;
//import android.os.Bundle;
//import android.os.Parcelable;
//import android.os.SystemClock;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.util.SparseArray;
//import android.view.View;
//import android.widget.GridView;

//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorSet;
//import com.nineoldandroids.view.ViewHelper;

/**
 * A class which decides whether given Views should be animated based on their position: each View should only be animated once.
 * It also calculates proper animation delays for the views.
 */

using Android.Animation;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;

namespace Com.Nhaarman.ListviewAnimations.Appearance
{
    public class ViewAnimator
    {

        /* Saved instance state keys */
        private static readonly string SAVEDINSTANCESTATE_FIRSTANIMATEDPOSITION = "savedinstancestate_firstanimatedposition";
        private static readonly string SAVEDINSTANCESTATE_LASTANIMATEDPOSITION = "savedinstancestate_lastanimatedposition";
        private static readonly string SAVEDINSTANCESTATE_SHOULDANIMATE = "savedinstancestate_shouldanimate";

        /* Default values */

        /**
         * The default delay in millis before the first animation starts.
         */
        private static readonly int INITIAL_DELAY_MILLIS = 150;

        /**
         * The default delay in millis between view animations.
         */
        private static readonly int DEFAULT_ANIMATION_DELAY_MILLIS = 100;

        /**
         * The default duration in millis of the animations.
         */
        private static readonly int DEFAULT_ANIMATION_DURATION_MILLIS = 300;

        /* Fields */

        /**
         * The ListViewWrapper containing the ListView implementation.
         */
        //@NonNull
        private readonly IListViewWrapper mListViewWrapper;

        /**
         * The active Animators. Keys are hashcodes of the Views that are animated.
         */
        //@NonNull
        private readonly SparseArray<Animator> mAnimators = new SparseArray<Animator>();

        /**
         * The delay in millis before the first animation starts.
         */
        private int mInitialDelayMillis = INITIAL_DELAY_MILLIS;

        /**
         * The delay in millis between view animations.
         */
        private int mAnimationDelayMillis = DEFAULT_ANIMATION_DELAY_MILLIS;

        /**
         * The duration in millis of the animations.
         */
        private int mAnimationDurationMillis = DEFAULT_ANIMATION_DURATION_MILLIS;

        /**
         * The start timestamp of the first animation, as returned by {@link android.os.SystemClock#uptimeMillis()}.
         */
        private long mAnimationStartMillis;

        /**
         * The position of the item that is the first that was animated.
         */
        private int mFirstAnimatedPosition;

        /**
         * The position of the last item that was animated.
         */
        private int mLastAnimatedPosition;

        /**
         * Whether animation is enabled. When this is set to false, no animation is applied to the views.
         */
        private bool mShouldAnimate = true;

        /**
         * Creates a new ViewAnimator, using the given {@link com.nhaarman.listviewanimations.util.ListViewWrapper}.
         *
         * @param listViewWrapper the {@code ListViewWrapper} which wraps the implementation of the ListView used.
         */
        ////@NonNull readonly
        public ViewAnimator(IListViewWrapper listViewWrapper)
        {
            mListViewWrapper = listViewWrapper;
            mAnimationStartMillis = -1;
            mFirstAnimatedPosition = -1;
            mLastAnimatedPosition = -1;
        }

        /**
         * Call this method to reset animation status on all views.
         */
        public void reset()
        {
            for (int i = 0; i < mAnimators.Size(); i++)
            {
                mAnimators.Get(mAnimators.KeyAt(i)).Cancel();
            }
            mAnimators.Clear();
            mFirstAnimatedPosition = -1;
            mLastAnimatedPosition = -1;
            mAnimationStartMillis = -1;
            mShouldAnimate = true;
        }

        /**
         * Set the starting position for which items should animate. Given position will animate as well.
         * Will also call {@link #enableAnimations()}.
         *
         * @param position the position.
         */
        public void setShouldAnimateFromPosition(int position)
        {
            enableAnimations();
            mFirstAnimatedPosition = position - 1;
            mLastAnimatedPosition = position - 1;
        }

        /**
         * Set the starting position for which items should animate as the first position which isn't currently visible on screen. This call is also valid when the {@link View}s
         * haven't been drawn yet. Will also call {@link #enableAnimations()}.
         */
        public void setShouldAnimateNotVisible()
        {
            enableAnimations();
            mFirstAnimatedPosition = mListViewWrapper.getLastVisiblePosition();
            mLastAnimatedPosition = mListViewWrapper.getLastVisiblePosition();
        }

        /**
         * Sets the value of the last animated position. Views with positions smaller than or equal to given value will not be animated.
         */
        public void setLastAnimatedPosition(int lastAnimatedPosition)
        {
            mLastAnimatedPosition = lastAnimatedPosition;
        }

        /**
         * Sets the delay in milliseconds before the first animation should start. Defaults to {@value #INITIAL_DELAY_MILLIS}.
         *
         * @param delayMillis the time in milliseconds.
         */
        public void setInitialDelayMillis(int delayMillis)
        {
            mInitialDelayMillis = delayMillis;
        }

        /**
         * Sets the delay in milliseconds before an animation of a view should start. Defaults to {@value #DEFAULT_ANIMATION_DELAY_MILLIS}.
         *
         * @param delayMillis the time in milliseconds.
         */
        public void setAnimationDelayMillis(int delayMillis)
        {
            mAnimationDelayMillis = delayMillis;
        }

        /**
         * Sets the duration of the animation in milliseconds. Defaults to {@value #DEFAULT_ANIMATION_DURATION_MILLIS}.
         *
         * @param durationMillis the time in milliseconds.
         */
        public void setAnimationDurationMillis(int durationMillis)
        {
            mAnimationDurationMillis = durationMillis;
        }

        /**
         * Enables animating the Views. This is the default.
         */
        public void enableAnimations()
        {
            mShouldAnimate = true;
        }

        /**
         * Disables animating the Views. Enable them again using {@link #enableAnimations()}.
         */
        public void disableAnimations()
        {
            mShouldAnimate = false;
        }

        /**
         * Cancels any existing animations for given View.
         */
        ////@NonNull readonly 
        public void cancelExistingAnimation(View view)
        {
            int hashCode = view.GetHashCode();

            Animator animator = mAnimators.Get(hashCode);
            if (animator != null)
            {
                animator.End();
                mAnimators.Remove(hashCode);
            }
        }

        /**
         * Animates given View if necessary.
         *
         * @param position the position of the item the View represents.
         * @param view     the View that should be animated.
         */
        public void animateViewIfNecessary(int position, View view, Animator[] animators)
        {
            if (mShouldAnimate && position > mLastAnimatedPosition)
            {
                if (mFirstAnimatedPosition == -1)
                {
                    mFirstAnimatedPosition = position;
                }

                animateView(position, view, animators);
                mLastAnimatedPosition = position;
            }
        }

        /**
         * Animates given View.
         *
         * @param view the View that should be animated.
         */
        private void animateView(int position, View view, Animator[] animators)
        {
            if (mAnimationStartMillis == -1)
            {
                mAnimationStartMillis = SystemClock.UptimeMillis();
            }           
            //        ViewHelper.setAlpha(view, 0);

            view.Alpha = 0;
            
            AnimatorSet set = new AnimatorSet();
            set.PlayTogether(animators);
            set.StartDelay = calculateAnimationDelay(position);
            set.SetDuration(mAnimationDurationMillis);
            set.Start();

            mAnimators.Put(view.GetHashCode(), set);
        }

        /**
         * Returns the delay in milliseconds after which animation for View with position mLastAnimatedPosition + 1 should start.
         */
        //@SuppressLint("NewApi")
        private int calculateAnimationDelay(int position)
        {
            int delay;

            int lastVisiblePosition = mListViewWrapper.getLastVisiblePosition();
            int firstVisiblePosition = mListViewWrapper.getFirstVisiblePosition();

            int numberOfItemsOnScreen = lastVisiblePosition - firstVisiblePosition;
            int numberOfAnimatedItems = position - 1 - mFirstAnimatedPosition;

            if (numberOfItemsOnScreen + 1 < numberOfAnimatedItems)
            {
                delay = mAnimationDelayMillis;

                if (mListViewWrapper.getListView() is GridView && Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
                {
                    int numColumns = ((GridView)mListViewWrapper.getListView()).NumColumns;
                    delay += mAnimationDelayMillis * (position % numColumns);
                }
            }
            else
            {
                int delaySinceStart = (position - mFirstAnimatedPosition) * mAnimationDelayMillis;
                delay = Math.Max(0, (int)(-SystemClock.UptimeMillis() + mAnimationStartMillis + mInitialDelayMillis + delaySinceStart));
            }
            return delay;
        }

        /**
         * Returns a Parcelable object containing the AnimationAdapter's current dynamic state.
         */
        //@NonNull
        public IParcelable onSaveInstanceState()
        {
            Bundle bundle = new Bundle();

            bundle.PutInt(SAVEDINSTANCESTATE_FIRSTANIMATEDPOSITION, mFirstAnimatedPosition);
            bundle.PutInt(SAVEDINSTANCESTATE_LASTANIMATEDPOSITION, mLastAnimatedPosition);
            bundle.PutBoolean(SAVEDINSTANCESTATE_SHOULDANIMATE, mShouldAnimate);

            return bundle;
        }

        /**
         * Restores this AnimationAdapter's state.
         *
         * @param parcelable the Parcelable object previously returned by {@link #onSaveInstanceState()}.
         */
        public void onRestoreInstanceState(IParcelable parcelable)
        {
            if (parcelable is Bundle)
            {
                Bundle bundle = (Bundle)parcelable;
                mFirstAnimatedPosition = bundle.GetInt(SAVEDINSTANCESTATE_FIRSTANIMATEDPOSITION);
                mLastAnimatedPosition = bundle.GetInt(SAVEDINSTANCESTATE_LASTANIMATEDPOSITION);
                mShouldAnimate = bundle.GetBoolean(SAVEDINSTANCESTATE_SHOULDANIMATE);
            }
        }
    }
}