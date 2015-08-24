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

//package com.nhaarman.listviewanimations.itemmanipulation.animateaddition;

//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.util.Pair;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.AbsListView;
//import android.widget.AdapterView;
//import android.widget.BaseAdapter;
//import android.widget.ListView;

//import com.nhaarman.listviewanimations.BaseAdapterDecorator;
//import com.nhaarman.listviewanimations.util.AbsListViewWrapper;
//import com.nhaarman.listviewanimations.util.Insertable;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorListenerAdapter;
//import com.nineoldandroids.animation.AnimatorSet;
//import com.nineoldandroids.animation.ObjectAnimator;
//import com.nineoldandroids.animation.ValueAnimator;
//import com.nineoldandroids.view.ViewHelper;

//import java.util.ArrayList;
//import java.util.Arrays;
//import java.util.Collection;

/**
 * An adapter for inserting rows into the {@link android.widget.ListView} with an animation. The root {@link android.widget.BaseAdapter} should implement {@link Insertable},
 * otherwise an {@link IllegalArgumentException} is thrown. This class only works with an instance of {@code ListView}!
 * <p/>
 * Usage:<br>
 * - Wrap a new instance of this class around a {@link android.widget.BaseAdapter}. <br>
 * - Set a {@code ListView} to this class using {@link #setListView(android.widget.ListView)}.<br>
 * - Call {@link AnimateAdditionAdapter#insert(int, Object)} to animate the addition of an item.
 * <p/>
 * Extend this class and override {@link AnimateAdditionAdapter#getAdditionalAnimators(android.view.View,
 * android.view.ViewGroup)} to provide extra {@link com.nineoldandroids.animation.Animator}s.
 */

using Android.Animation;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;
using System.Collections.Generic;

namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.Animateaddition
{
    public class AnimateAdditionAdapter<T> : BaseAdapterDecorator
    {

        private static readonly long DEFAULT_SCROLLDOWN_ANIMATION_MS = 300;

        private long mScrolldownAnimationDurationMs = DEFAULT_SCROLLDOWN_ANIMATION_MS;

        private static readonly long DEFAULT_INSERTION_ANIMATION_MS = 300;

        private long mInsertionAnimationDurationMs = DEFAULT_INSERTION_ANIMATION_MS;

        private static readonly string ALPHA = "alpha";

        //@NonNull
        private Insertable mInsertable;

        //@NonNull
        private InsertQueue<T> mInsertQueue;

        /**
         * Describes whether the list should animate downwards when items are added above the first visible item.
         */
        private bool mShouldAnimateDown = true;

        /**
         * Create a new {@code AnimateAdditionAdapter} with given {@link android.widget.BaseAdapter}.
         *
         * @param baseAdapter should implement {@link Insertable},
         *                    or be a {@link com.nhaarman.listviewanimations.BaseAdapterDecorator} whose BaseAdapter implements the interface.
         */
        public AnimateAdditionAdapter(BaseAdapter baseAdapter)
            : base(baseAdapter)
        {
            //super(baseAdapter);

            BaseAdapter rootAdapter = getRootAdapter();
            if (!(rootAdapter is Insertable))
            {
                throw new Java.Lang.IllegalArgumentException("BaseAdapter should implement Insertable!");
            }
            //throw new NotImplementedException();
            //mInsertable = (Insertable<T>) rootAdapter;
            mInsertable = (Insertable)rootAdapter;
            mInsertQueue = new InsertQueue<T>(mInsertable);
        }

        /**
         * @deprecated use {@link #setListView(android.widget.ListView)} instead.
         */
        //@Override
        //@Deprecated
        public   void setAbsListView(AbsListView absListView)
        {
            if (absListView is ListView)
            {
                setListView((ListView)absListView);
            }
            else
            {
                throw new Java.Lang.IllegalArgumentException("AnimateAdditionAdapter requires a ListView!");
            }
        }

        /**
         * Sets the {@link android.widget.ListView} that is used for this {@code AnimateAdditionAdapter}.
         */
        public void setListView(ListView listView)
        {
            setListViewWrapper(new AbsListViewWrapper(listView));
        }

        /**
         * Sets whether the list should animate downwards when items are added above the first visible item.
         *
         * @param shouldAnimateDown defaults to {@code true}.
         */
        //@SuppressWarnings("UnusedDeclaration")
        public void setShouldAnimateDown(bool shouldAnimateDown)
        {
            mShouldAnimateDown = shouldAnimateDown;
        }

        /**
         * Set the duration of the scrolldown animation <i>per item</i> for when items are inserted above the first visible item.
         *
         * @param scrolldownAnimationDurationMs the duration in ms.
         */
        //@SuppressWarnings("UnusedDeclaration")
        public void setScrolldownAnimationDuration(long scrolldownAnimationDurationMs)
        {
            mScrolldownAnimationDurationMs = scrolldownAnimationDurationMs;
        }

        /**
         * Set the duration of the insertion animation.
         *
         * @param insertionAnimationDurationMs the duration in ms.
         */
        //@SuppressWarnings("UnusedDeclaration")
        public void setInsertionAnimationDuration(long insertionAnimationDurationMs)
        {
            mInsertionAnimationDurationMs = insertionAnimationDurationMs;
        }

        /**
         * Inserts an item at given index. Will show an entrance animation for the new item if the newly added item is visible.
         * Will also call {@link Insertable#add(int, Object)} of the root {@link android.widget.BaseAdapter}.
         *
         * @param index the index the new item should be inserted at.
         * @param item  the item to insert.
         */
        public void insert(int index, T item)
        {
            insert(new KeyValuePair<int, T>(index, item));
        }

        /**
         * Inserts items, starting at given index. Will show an entrance animation for the new items if the newly added items are visible.
         * Will also call {@link Insertable#add(int, Object)} of the root {@link android.widget.BaseAdapter}.
         *
         * @param index the starting index the new items should be inserted at.
         * @param items the items to insert.
         */
        public void insert(int index, params T[] items)
        {
            KeyValuePair<int, T>[] pairs = new KeyValuePair<int, T>[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                pairs[i] = new KeyValuePair<int, T>(index + i, items[i]);
            }
            insert(pairs);
        }

        /**
         * Inserts items at given indexes. Will show an entrance animation for the new items if the newly added item is visible.
         * Will also call {@link Insertable#add(int, Object)} of the root {@link android.widget.BaseAdapter}.
         *
         * @param indexItemPairs the index-item pairs to insert. The first argument of the {@code Pair} is the index, the second argument is the item.
         */
        public void insert(params KeyValuePair<int, T>[] indexItemPairs)
        {
            //throw new NotImplementedException();
            //insert(indexItemPairs as IEnumerable<int,T>);

            insert((IEnumerable<KeyValuePair<int, T>>)indexItemPairs);
        }

        /**
         * Inserts items at given indexes. Will show an entrance animation for the new items if the newly added item is visible.
         * Will also call {@link Insertable#add(int, Object)} of the root {@link android.widget.BaseAdapter}.
         *
         * @param indexItemPairs the index-item pairs to insert. The first argument of the {@code Pair} is the index, the second argument is the item.
         */
        public void insert(IEnumerable<KeyValuePair<int, T>> indexItemPairs)
        {
            if (getListViewWrapper() == null)
            {
                throw new Java.Lang.IllegalStateException("Call setListView on this AnimateAdditionAdapter!");
            }

            ICollection<KeyValuePair<int, T>> visibleViews = new List<KeyValuePair<int, T>>();
            ICollection<int> insertedPositions = new List<int>();
            ICollection<int> insertedBelowPositions = new List<int>();

            int scrollDistance = 0;
            int numInsertedAbove = 0;

            foreach (KeyValuePair<int, T> pair in indexItemPairs)
            {
                if (getListViewWrapper().getFirstVisiblePosition() > pair.Key)
                {
                    /* Inserting an item above the first visible position */
                    int index = pair.Key;

                    /* Correct the index for already inserted positions above the first visible view. */
                    foreach (int insertedPosition in insertedPositions)
                    {
                        if (index >= insertedPosition)
                        {
                            index++;
                        }
                    }

                    mInsertable.add(index, pair.Value);
                    insertedPositions.Add(index);
                    numInsertedAbove++;

                    if (mShouldAnimateDown)
                    {
                        View view = GetView(pair.Key, null, getListViewWrapper().getListView());

                        view.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                        scrollDistance -= view.MeasuredHeight;
                    }
                }
                else if (getListViewWrapper().getLastVisiblePosition() >= pair.Key || getListViewWrapper().getLastVisiblePosition() == AdapterView.InvalidPosition ||
                      !childrenFillAbsListView())
                {
                    /* Inserting an item that becomes visible on screen */
                    int index = pair.Key;

                    /* Correct the index for already inserted positions above the first visible view */
                    foreach (int insertedPosition in insertedPositions)
                    {
                        if (index >= insertedPosition)
                        {
                            index++;
                        }
                    }
                    KeyValuePair<int, T> newPair = new KeyValuePair<int, T>(index, pair.Value);
                    visibleViews.Add(newPair);
                }
                else
                {
                    /* Inserting an item below the last visible item */
                    int index = pair.Key;

                    /* Correct the index for already inserted positions above the first visible view */
                    foreach (int insertedPosition in insertedPositions)
                    {
                        if (index >= insertedPosition)
                        {
                            index++;
                        }
                    }

                    /* Correct the index for already inserted positions below the last visible view */
                    foreach (int queuedPosition in insertedBelowPositions)
                    {
                        if (index >= queuedPosition)
                        {
                            index++;
                        }
                    }

                    insertedBelowPositions.Add(index);
                    mInsertable.add(index, pair.Value);
                }
            }

            if (mShouldAnimateDown)
            {
                ((AbsListView)getListViewWrapper().getListView()).SmoothScrollBy(scrollDistance, (int)(mScrolldownAnimationDurationMs * numInsertedAbove));
            }

            mInsertQueue.insert(visibleViews);

            int firstVisiblePosition = getListViewWrapper().getFirstVisiblePosition();
            View firstChild = getListViewWrapper().getChildAt(0);
            int childTop = firstChild == null ? 0 : firstChild.Top;
            ((ListView)getListViewWrapper().getListView()).SetSelectionFromTop(firstVisiblePosition + numInsertedAbove, childTop);
        }

        /**
         * @return true if the children completely fill up the AbsListView.
         */
        private bool childrenFillAbsListView()
        {
            if (getListViewWrapper() == null)
            {
                throw new Java.Lang.IllegalStateException("Call setListView on this AnimateAdditionAdapter first!");
            }

            int childrenHeight = 0;
            for (int i = 0; i < getListViewWrapper().getCount(); i++)
            {
                View child = getListViewWrapper().getChildAt(i);
                if (child != null)
                {
                    childrenHeight += child.Height;
                }
            }
            return getListViewWrapper().getListView().Height <= childrenHeight;
        }

        //@Override
        //@NonNull
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = base.GetView(position, convertView, parent);

            if (mInsertQueue.getActiveIndexes().Contains(position))
            {
                int widthMeasureSpec = View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.MatchParent, MeasureSpecMode.AtMost);
                int heightMeasureSpec = View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified);
                view.Measure(widthMeasureSpec, heightMeasureSpec);

                int originalHeight = view.MeasuredHeight;

                ValueAnimator heightAnimator = ValueAnimator.OfInt(1, originalHeight);
                heightAnimator.AddUpdateListener(new HeightUpdater(view));

                Animator[] customAnimators = getAdditionalAnimators(view, parent);
                Animator[] animators = new Animator[customAnimators.Length + 1];
                animators[0] = heightAnimator;

                //System.arraycopy(customAnimators, 0, animators, 1, customAnimators.length);
                Array.Copy(customAnimators, 0, animators, 1, customAnimators.Length);

                AnimatorSet animatorSet = new AnimatorSet();
                animatorSet.PlayTogether(animators);

                //ViewHelper.setAlpha(view, 0);
                view.Alpha = 0;
                ObjectAnimator alphaAnimator = ObjectAnimator.OfFloat(view, ALPHA, 0, 1);

                AnimatorSet allAnimatorsSet = new AnimatorSet();
                allAnimatorsSet.PlaySequentially(animatorSet, alphaAnimator);

                allAnimatorsSet.SetDuration(mInsertionAnimationDurationMs);
                allAnimatorsSet.AddListener(new ExpandAnimationListener(this, position));
                allAnimatorsSet.Start();
            }

            return view;
        }

        /**
         * Override this method to provide additional animators on top of the default height and alpha animation.
         *
         * @param view   The {@link android.view.View} that will get animated.
         * @param parent The parent that this view will eventually be attached to.
         *
         * @return a non-null array of Animators.
         */
        //@SuppressWarnings({"MethodMayBeStatic", "UnusedParameters"})
        //@NonNull
        protected Animator[] getAdditionalAnimators(View view, ViewGroup parent)
        {
            return new Animator[] { };
        }

        /**
         * A class which applies the animated height value to a {@code View}.
         */
        private class HeightUpdater :Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
        {

            private View mView;

            public HeightUpdater(View view)
            {
                mView = view;
            }

            //@Override
            public void OnAnimationUpdate(ValueAnimator animation)
            {
                ViewGroup.LayoutParams layoutParams = mView.LayoutParameters;
                layoutParams.Height = (int)animation.AnimatedValue;
                mView.LayoutParameters = layoutParams;
            }
        }

        /**
         * A class which removes the active index from the {@code InsertQueue} when the animation has finished.
         */
        private class ExpandAnimationListener : AnimatorListenerAdapter
        {

            private int mPosition;
            private AnimateAdditionAdapter<T> mAdapter;

            public ExpandAnimationListener(AnimateAdditionAdapter<T> adapter, int position)
            {
                mAdapter = adapter;
                mPosition = position;
            }

            //@Override
            public override void OnAnimationEnd(Animator animation)
            {
                mAdapter.mInsertQueue.removeActiveIndex(mPosition);
            }
        }
    }
}