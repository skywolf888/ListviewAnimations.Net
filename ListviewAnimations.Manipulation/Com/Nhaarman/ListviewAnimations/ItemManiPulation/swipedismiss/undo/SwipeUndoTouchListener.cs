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
//import android.view.View;
//import android.view.ViewGroup;

//import com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.SwipeDismissTouchListener;
//import com.nhaarman.listviewanimations.util.AdapterViewUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorListenerAdapter;
//import com.nineoldandroids.animation.AnimatorSet;
//import com.nineoldandroids.animation.ObjectAnimator;

using Android.Animation;
using Android.Views;
using Com.Nhaarman.ListviewAnimations.Util;
//import java.util.Collection;
//import java.util.HashMap;
//import java.util.LinkedList;
//import java.util.List;
//import java.util.Map;
using System.Collections.Generic;


namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{
/**
 * A {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.SwipeDismissTouchListener} that adds an undo stage to the item swiping.
 */
public class SwipeUndoTouchListener : SwipeDismissTouchListener {

    private static readonly string ALPHA = "alpha";

    private static readonly string TRANSLATION_X = "translationX";

    /**
     * The callback which gets notified of events.
     */
    //@NonNull
    private   IUndoCallback mCallback;

    /**
     * The positions that are in the undo state.
     */
    //@NonNull
    private   ICollection<int> mUndoPositions = new List<int>();

    /**
     * The {@link android.view.View}s that are in the undo state.
     */
    //@NonNull

	 
    private   Dictionary<int, View> mUndoViews = new Dictionary<int,View>();

    /**
     * The positions that have been dismissed.
     */
    //@NonNull
    private   List<int> mDismissedPositions = new List<int>();

    /**
     * The {@link android.view.View}s that have been dismissed.
     */
    //@NonNull
    private   ICollection<View> mDismissedViews = new LinkedList<View>();

    public SwipeUndoTouchListener( IListViewWrapper listViewWrapper,   IUndoCallback callback) 
		:base(listViewWrapper, callback)

    {
        
        mCallback = callback;
    }

    //@Override
    protected override bool willLeaveDataSetOnFling( View view,   int position) {
        return mUndoPositions.Contains(position);
    }

    //@Override
    protected override void afterViewFling( View view,   int position) {
        if (mUndoPositions.Contains(position)) {
            mUndoPositions.Remove(position);
            mUndoViews.Remove(position);
            performDismiss(view, position);
            hideUndoView(view);
        } else {
            mUndoPositions.Add(position);
            mUndoViews.Add(position, view);
            mCallback.onUndoShown(view, position);
            showUndoView(view);
            restoreViewPresentation(view);
        }
    }

    //@Override
    protected override void afterCancelSwipe( View view,   int position) {
        finalizeDismiss();
    }

    /**
     * Animates the dismissed list item to zero-height and fires the dismiss callback when all dismissed list item animations have completed.
     *
     * @param view the dismissed {@link android.view.View}.
     */
    //@Override
    protected  override void performDismiss( View view,  int position) {
        base.performDismiss(view, position);

        mDismissedViews.Add(view);
        mDismissedPositions.Add(position);

        mCallback.onDismiss(view, position);
    }

    public bool hasPendingItems() {
        return mUndoPositions.Count!=0;
    }

    /**
     * Dismisses all items that are in the undo state.
     */
    public void dimissPending() {
        foreach (int position in mUndoPositions) {
            performDismiss(mUndoViews[position], position);
        }
    }

    /**
     * Sets the visibility of the primary {@link android.view.View} to {@link android.view.View#GONE}, and animates the undo {@code View} in to view.
     *
     * @param view the parent {@code View} which contains both primary and undo {@code View}s.
     */
    private void showUndoView( View view) {
        mCallback.getPrimaryView(view).Visibility=ViewStates.Gone;

        View undoView = mCallback.getUndoView(view);
        undoView.Visibility=ViewStates.Visible;
        ObjectAnimator.OfFloat(undoView, ALPHA, 0f, 1f).Start();
    }

    /**
     * Sets the visibility of the primary {@link android.view.View} to {@link android.view.View#VISIBLE}, and that of the undo {@code View} to {@link android.view.View#GONE}.
     *
     * @param view the parent {@code View} which contains both primary and undo {@code View}s.
     */
    private void hideUndoView( View view) {
        mCallback.getPrimaryView(view).Visibility=ViewStates.Visible;
        mCallback.getUndoView(view).Visibility=ViewStates.Gone;
    }


    /**
     * If necessary, notifies the {@link UndoCallback} to remove dismissed object from the adapter,
     * and restores the {@link android.view.View} presentations.
     */
    //@Override
    protected override void finalizeDismiss() {
         
        if (getActiveDismissCount() == 0 && getActiveSwipeCount() == 0) {
            restoreViewPresentations(mDismissedViews);
            notifyCallback(mDismissedPositions);

            ICollection<int> newUndoPositions = Util.processDeletions(mUndoPositions, mDismissedPositions);
            mUndoPositions.Clear();
            //mUndoPositions.addAll(newUndoPositions);
            (mUndoPositions as List<int>).AddRange(newUndoPositions);
            mDismissedViews.Clear();
            mDismissedPositions.Clear();
        }
    }

    /**
     * Restores the height of given {@code View}.
     * Also calls its super implementation.
     */
    //@Override
    protected void restoreViewPresentation( View view) {
        base.restoreViewPresentation(view);
        ViewGroup.LayoutParams layoutParams = view.LayoutParameters;
        layoutParams.Height = 0;
        view.LayoutParameters=layoutParams;
    }

    /**
     * Performs the undo animation and restores the original state for given {@link android.view.View}.
     *
     * @param view the parent {@code View} which contains both primary and undo {@code View}s.
     */
    public void undo( View view) {
        int position = AdapterViewUtil.getPositionForView(getListViewWrapper(), view);
        mUndoPositions.Remove(position);

        View primaryView = mCallback.getPrimaryView(view);
        View undoView = mCallback.getUndoView(view);

        primaryView.Visibility=ViewStates.Visible;

        ObjectAnimator undoAlphaAnimator = ObjectAnimator.OfFloat(undoView, ALPHA, 1f, 0f);
        ObjectAnimator primaryAlphaAnimator = ObjectAnimator.OfFloat(primaryView, ALPHA, 0f, 1f);
        ObjectAnimator primaryXAnimator = ObjectAnimator.OfFloat(primaryView, TRANSLATION_X, primaryView.Width, 0f);

        AnimatorSet animatorSet = new AnimatorSet();
        animatorSet.PlayTogether(undoAlphaAnimator, primaryAlphaAnimator, primaryXAnimator);
        animatorSet.AddListener(new UndoAnimatorListener(undoView,this));
        animatorSet.Start();

        mCallback.onUndo(view, position);
    }

    //@Override
    protected   void directDismiss(  int position) {
        mDismissedPositions.Add(position);
        finalizeDismiss();
    }

    /**
     * An {@link com.nineoldandroids.animation.Animator.AnimatorListener} which finalizes the undo when the animation is finished.
     */
    private class UndoAnimatorListener : AnimatorListenerAdapter {

        //@NonNull
        private   View mUndoView;
        private SwipeUndoTouchListener minst;

        public UndoAnimatorListener( View undoView,SwipeUndoTouchListener instance) {
            mUndoView = undoView;
            minst = instance;
        }

        //@Override
        public void onAnimationEnd( Animator animation) {
            mUndoView.Visibility=ViewStates.Gone;
            minst.finalizeDismiss();
        }
    }
}
}