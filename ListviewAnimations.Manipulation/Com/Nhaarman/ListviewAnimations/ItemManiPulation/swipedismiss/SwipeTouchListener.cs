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
/* Originally based on Roman Nurik's SwipeDismissListViewTouchListener (https://gist.github.com/romannurik/2980593). */
//package com.nhaarman.listviewanimations.itemmanipulation.swipedismiss;

//import android.graphics.Rect;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.MotionEvent;
//import android.view.VelocityTracker;
//import android.view.View;
//import android.view.ViewConfiguration;
//import android.view.ViewGroup;
//import android.widget.AdapterView;

using Android.Animation;
using Android.Graphics;
//import com.nhaarman.listviewanimations.itemmanipulation.TouchEventHandler;
//import com.nhaarman.listviewanimations.util.AdapterViewUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorListenerAdapter;
//import com.nineoldandroids.animation.AnimatorSet;
//import com.nineoldandroids.animation.ObjectAnimator;
//import com.nineoldandroids.view.ViewHelper;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss
{

    /**
     * An {@link android.view.View.OnTouchListener} that makes the list items in a {@link android.widget.AbsListView} swipeable.
     * Implementations of this class should implement {@link #afterViewFling(android.view.View, int)} to specify what to do after an item has been swiped.
     */
    public abstract class SwipeTouchListener :Java.Lang.Object, View.IOnTouchListener, TouchEventHandler
    {

        /**
         * TranslationX View property.
         */
        private static readonly string TRANSLATION_X = "translationX";

        /**
         * Alpha View property.
         */
        private static readonly string ALPHA = "alpha";

        private static readonly int MIN_FLING_VELOCITY_FACTOR = 16;

        /**
         * The minimum distance in pixels that should be moved before starting horizontal item movement.
         */
        private int mSlop;

        /**
         * The minimum velocity to initiate a fling, as measured in pixels per second.
         */
        private int mMinFlingVelocity;

        /**
         * The maximum velocity to initiate a fling, as measured in pixels per second.
         */
        private int mMaxFlingVelocity;

        /**
         * The duration of the fling animation.
         */
        private long mAnimationTime;

        //@NonNull
        private ListViewWrapper mListViewWrapper;

        /**
         * The minimum alpha value of swiped Views.
         */
        private float mMinimumAlpha;

        /**
         * The width of the {@link android.widget.AbsListView} in pixels.
         */
        private int mViewWidth = 1;

        /**
         * The raw X coordinate of the down event.
         */
        private float mDownX;

        /**
         * The raw Y coordinate of the down event.
         */
        private float mDownY;

        /**
         * Indicates whether the user is swiping an item.
         */
        private bool mSwiping;

        /**
         * Indicates whether the user can dismiss the current item.
         */
        private bool mCanDismissCurrent;

        /**
         * The {@code VelocityTracker} used in the swipe movement.
         */
        //@Nullable
        private VelocityTracker mVelocityTracker;

        /**
         * The parent {@link android.view.View} being swiped.
         */
        //@Nullable
        private View mCurrentView;

        /**
         * The {@link android.view.View} that is actually being swiped.
         */
        //@Nullable
        private View mSwipingView;

        /**
         * The current position being swiped.
         */
        private int mCurrentPosition = AdapterView.InvalidPosition;

        /**
         * The number of items in the {@code AbsListView}, minus the pending dismissed items.
         */
        private int mVirtualListCount = -1;

        /**
         * Indicates whether the {@link android.widget.AbsListView} is in a horizontal scroll container.
         * If so, this class will prevent the horizontal scroller from receiving any touch events.
         */
        private bool mParentIsHorizontalScrollContainer;

        /**
         * The resource id of the {@link android.view.View} that may steal touch events from their parents. Useful for example
         * when the {@link android.widget.AbsListView} is in a horizontal scroll container, but not the whole {@code AbsListView} should
         * steal the touch events.
         */
        private int mTouchChildResId;

        /**
         * The {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.DismissableManager} which decides
         * whether or not a list item can be swiped.
         */
        //@Nullable
        private DismissableManager mDismissableManager;

        /**
         * The number of active swipe animations.
         */
        private int mActiveSwipeCount;

        /**
         * Indicates whether swipe is enabled.
         */
        private bool mSwipeEnabled = true;

        /**
         * Constructs a new {@code SwipeTouchListener} for the given {@link android.widget.AbsListView}.
         */
        //@SuppressWarnings("UnnecessaryFullyQualifiedName")
        protected SwipeTouchListener(ListViewWrapper listViewWrapper)
        {
            ViewConfiguration vc = ViewConfiguration.Get(listViewWrapper.getListView().Context);
            mSlop = vc.ScaledTouchSlop;
            mMinFlingVelocity = vc.ScaledMinimumFlingVelocity * MIN_FLING_VELOCITY_FACTOR;
            mMaxFlingVelocity = vc.ScaledMaximumFlingVelocity;
            mAnimationTime = listViewWrapper.getListView().Context.Resources.GetInteger(Android.Resource.Integer.ConfigShortAnimTime);
            mListViewWrapper = listViewWrapper;
        }

        /**
         * Sets the {@link com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.DismissableManager} to specify which views can or cannot be swiped.
         *
         * @param dismissableManager {@code null} for no restrictions.
         */
        public void setDismissableManager(DismissableManager dismissableManager)
        {
            mDismissableManager = dismissableManager;
        }

        /**
         * Set the minimum value of the alpha property swiping Views should have.
         *
         * @param minimumAlpha the alpha value between 0.0f and 1.0f.
         */
        public void setMinimumAlpha(float minimumAlpha)
        {
            mMinimumAlpha = minimumAlpha;
        }

        /**
         * If the {@link android.widget.AbsListView} is hosted inside a parent(/grand-parent/etc) that can scroll horizontally, horizontal swipes won't
         * work, because the parent will prevent touch-events from reaching the {@code AbsListView}.
         * <p/>
         * Call this method to fix this behavior.
         * Note that this will prevent the parent from scrolling horizontally when the user touches anywhere in a list item.
         */
        public void setParentIsHorizontalScrollContainer()
        {
            mParentIsHorizontalScrollContainer = true;
            mTouchChildResId = 0;
        }

        /**
         * Sets the resource id of a child view that should be touched to engage swipe.
         * When the user touches a region outside of that view, no swiping will occur.
         *
         * @param childResId The resource id of the list items' child that the user should touch to be able to swipe the list items.
         */
        public void setTouchChild(int childResId)
        {
            mTouchChildResId = childResId;
            mParentIsHorizontalScrollContainer = false;
        }

        /**
         * Notifies this {@code SwipeTouchListener} that the adapter contents have changed.
         */
        public void notifyDataSetChanged()
        {
            if (mListViewWrapper.getAdapter() != null)
            {
                mVirtualListCount = mListViewWrapper.getCount() - mListViewWrapper.getHeaderViewsCount();
            }
        }

        /**
         * Returns whether the user is currently swiping an item.
         *
         * @return {@code true} if the user is swiping an item.
         */
        public bool isSwiping()
        {
            return mSwiping;
        }

        //@NonNull
        public ListViewWrapper getListViewWrapper()
        {
            return mListViewWrapper;
        }

        /**
         * Enables the swipe behavior.
         */
        public void enableSwipe()
        {
            mSwipeEnabled = true;
        }

        /**
         * Disables the swipe behavior.
         */
        public void disableSwipe()
        {
            mSwipeEnabled = false;
        }

        /**
         * Flings the {@link android.view.View} corresponding to given position out of sight.
         * Calling this method has the same effect as manually swiping an item off the screen.
         *
         * @param position the position of the item in the {@link android.widget.ListAdapter}. Must be visible.
         */
        public virtual void fling(int position)
        {
            int firstVisiblePosition = mListViewWrapper.getFirstVisiblePosition();
            int lastVisiblePosition = mListViewWrapper.getLastVisiblePosition();
            if (position < firstVisiblePosition || position > lastVisiblePosition)
            {
                throw new Java.Lang.IllegalArgumentException("View for position " + position + " not visible!");
            }

            View downView = AdapterViewUtil.getViewForPosition(mListViewWrapper, position);
            if (downView == null)
            {
                throw new Java.Lang.IllegalStateException("No view found for position " + position);
            }
            flingView(downView, position, true);

            mActiveSwipeCount++;
            mVirtualListCount--;
        }

        //@Override
        public bool isInteracting()
        {
            return mSwiping;
        }

        //@Override
        public bool onTouchEvent(MotionEvent mevent)
        {
            return OnTouch(null, mevent);
        }

        //@Override
        public   bool OnTouch(View view, MotionEvent mevent)
        {
            if (mListViewWrapper.getAdapter() == null)
            {
                return false;
            }

            if (mVirtualListCount == -1 || mActiveSwipeCount == 0)
            {
                mVirtualListCount = mListViewWrapper.getCount() - mListViewWrapper.getHeaderViewsCount();
            }

            if (mViewWidth < 2)
            {
                mViewWidth = mListViewWrapper.getListView().Width;
            }

            bool result;
            switch (mevent.ActionMasked)
            {
                case MotionEventActions.Down:
                    result = handleDownEvent(view, mevent);
                    break;
                case MotionEventActions.Move:
                    result = handleMoveEvent(view, mevent);
                    break;
                case MotionEventActions.Cancel:
                    result = handleCancelEvent();
                    break;
                case MotionEventActions.Up:
                    result = handleUpEvent(mevent);
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        private bool handleDownEvent(View view, MotionEvent motionEvent)
        {
            if (!mSwipeEnabled)
            {
                return false;
            }

            View downView = findDownView(motionEvent);
            if (downView == null)
            {
                return false;
            }

            int downPosition = AdapterViewUtil.getPositionForView(mListViewWrapper, downView);
            mCanDismissCurrent = isDismissable(downPosition);

            /* Check if we are processing the item at this position */
            if (mCurrentPosition == downPosition || downPosition >= mVirtualListCount)
            {
                return false;
            }

            if (view != null)
            {
                view.OnTouchEvent(motionEvent);
            }

            disableHorizontalScrollContainerIfNecessary(motionEvent, downView);

            mDownX = motionEvent.GetX();
            mDownY = motionEvent.GetY();

            mCurrentView = downView;
            mSwipingView = getSwipeView(downView);
            mCurrentPosition = downPosition;

            mVelocityTracker = VelocityTracker.Obtain();
            mVelocityTracker.AddMovement(motionEvent);
            return true;
        }

        /**
         * Returns the child {@link android.view.View} that was touched, by performing a hit test.
         *
         * @param motionEvent the {@link android.view.MotionEvent} to find the {@code View} for.
         *
         * @return the touched {@code View}, or {@code null} if none found.
         */
        //@Nullable
        private View findDownView(MotionEvent motionEvent)
        {
            Rect rect = new Rect();
            int childCount = mListViewWrapper.getChildCount();
            int x = (int)motionEvent.GetX();
            int y = (int)motionEvent.GetY();
            View downView = null;
            for (int i = 0; i < childCount && downView == null; i++)
            {
                View child = mListViewWrapper.getChildAt(i);
                if (child != null)
                {
                    child.GetHitRect(rect);
                    if (rect.Contains(x, y))
                    {
                        downView = child;
                    }
                }
            }
            return downView;
        }

        /**
         * Finds out whether the item represented by given position is dismissable.
         *
         * @param position the position of the item.
         *
         * @return {@code true} if the item is dismissable, false otherwise.
         */
        private bool isDismissable(int position)
        {
            if (mListViewWrapper.getAdapter() == null)
            {
                return false;
            }

            if (mDismissableManager != null)
            {
                long downId = mListViewWrapper.getAdapter().GetItemId(position);
                return mDismissableManager.isDismissable(downId, position);
            }
            return true;
        }

        private void disableHorizontalScrollContainerIfNecessary(MotionEvent motionEvent, View view)
        {
            if (mParentIsHorizontalScrollContainer)
            {
                mListViewWrapper.getListView().RequestDisallowInterceptTouchEvent(true);
            }
            else if (mTouchChildResId != 0)
            {
                mParentIsHorizontalScrollContainer = false;

                View childView = view.FindViewById(mTouchChildResId);
                if (childView != null)
                {
                    Rect childRect = getChildViewRect(mListViewWrapper.getListView(), childView);
                    if (childRect.Contains((int)motionEvent.GetX(), (int)motionEvent.GetY()))
                    {
                        mListViewWrapper.getListView().RequestDisallowInterceptTouchEvent(true);
                    }
                }
            }
        }

        private bool handleMoveEvent(View view, MotionEvent motionEvent) {
        if (mVelocityTracker == null || mCurrentView == null) {
            return false;
        }

        mVelocityTracker.AddMovement(motionEvent);

        float deltaX = motionEvent.GetX() - mDownX;
        float deltaY = motionEvent.GetY() - mDownY;

        if (Math.Abs(deltaX) > mSlop && Math.Abs(deltaX) > Math.Abs(deltaY)) {
            if (!mSwiping) {
                mActiveSwipeCount++;
                onStartSwipe(mCurrentView, mCurrentPosition);
            }
            mSwiping = true;
            mListViewWrapper.getListView().RequestDisallowInterceptTouchEvent(true);

            /* Cancel ListView's touch (un-highlighting the item) */
            if (view != null) {
                MotionEvent cancelEvent = MotionEvent.Obtain(motionEvent);
                throw new NotImplementedException();
                //cancelEvent.Action=MotionEventActions. Cancel | motionEvent.ActionIndex << MotionEventActions.PointerIndexShift);
                view.OnTouchEvent(cancelEvent);
                cancelEvent.Recycle();
            }
        }

        if (mSwiping) {
            if (mCanDismissCurrent) {
				mSwipingView.TranslationX=deltaX;
                //ViewHelper.setTranslationX(mSwipingView, deltaX);
                //ViewHelper.setAlpha(mSwipingView, Math.Max(mMinimumAlpha, Math.Min(1, 1 - 2 * Math.Abs(deltaX) / mViewWidth)));
			    mSwipingView.Alpha=Math.Max(mMinimumAlpha, Math.Min(1, 1 - 2 * Math.Abs(deltaX) / mViewWidth));

            } else {
                //ViewHelper.setTranslationX(mSwipingView, deltaX * 0.1f);
				mSwipingView.TranslationX= deltaX * 0.1f;
            }
            return true;
        }
        return false;
    }

        private bool handleCancelEvent()
        {
            if (mVelocityTracker == null || mCurrentView == null)
            {
                return false;
            }

            if (mCurrentPosition != AdapterView.InvalidPosition && mSwiping)
            {
                onCancelSwipe(mCurrentView, mCurrentPosition);
                restoreCurrentViewTranslation();
            }

            reset();
            return false;
        }

        private bool handleUpEvent(MotionEvent motionEvent)
        {
            if (mVelocityTracker == null || mCurrentView == null)
            {
                return false;
            }

            if (mSwiping)
            {
                bool shouldDismiss = false;
                bool dismissToRight = false;

                if (mCanDismissCurrent)
                {
                    float deltaX = motionEvent.GetX() - mDownX;

                    mVelocityTracker.AddMovement(motionEvent);
                    mVelocityTracker.ComputeCurrentVelocity(1000);

                    float velocityX = Math.Abs(mVelocityTracker.XVelocity);
                    float velocityY = Math.Abs(mVelocityTracker.YVelocity);

                    if (Math.Abs(deltaX) > mViewWidth / 2)
                    {
                        shouldDismiss = true;
                        dismissToRight = deltaX > 0;
                    }
                    else if (mMinFlingVelocity <= velocityX && velocityX <= mMaxFlingVelocity && velocityY < velocityX)
                    {
                        shouldDismiss = true;
                        dismissToRight = mVelocityTracker.XVelocity > 0;
                    }
                }


                if (shouldDismiss)
                {
                    beforeViewFling(mCurrentView, mCurrentPosition);
                    if (willLeaveDataSetOnFling(mCurrentView, mCurrentPosition))
                    {
                        mVirtualListCount--;
                    }
                    flingCurrentView(dismissToRight);
                }
                else
                {
                    onCancelSwipe(mCurrentView, mCurrentPosition);
                    restoreCurrentViewTranslation();
                }
            }

            reset();
            return false;
        }

        /**
         * Flings the pending {@link android.view.View} out of sight.
         *
         * @param flingToRight {@code true} if the {@code View} should be flinged to the right, {@code false} if it should be flinged to the left.
         */
        private void flingCurrentView(bool flingToRight)
        {
            if (mCurrentView != null)
            {
                flingView(mCurrentView, mCurrentPosition, flingToRight);
            }
        }

        /**
         * Flings given {@link android.view.View} out of sight.
         *
         * @param view         the parent {@link android.view.View}.
         * @param position     the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         * @param flingToRight {@code true} if the {@code View} should be flinged to the right, {@code false} if it should be flinged to the left.
         */
        private void flingView(View view, int position, bool flingToRight)
        {
            if (mViewWidth < 2)
            {
                mViewWidth = mListViewWrapper.getListView().Width;
            }

            View swipeView = getSwipeView(view);
            ObjectAnimator xAnimator = ObjectAnimator.OfFloat(swipeView, TRANSLATION_X, flingToRight ? mViewWidth : -mViewWidth);
            ObjectAnimator alphaAnimator = ObjectAnimator.OfFloat(swipeView, ALPHA, 0);

            AnimatorSet animatorSet = new AnimatorSet();
            animatorSet.PlayTogether(xAnimator, alphaAnimator);
            animatorSet.SetDuration(mAnimationTime);
            animatorSet.AddListener(new FlingAnimatorListener(view, position,this));
            animatorSet.Start();
        }

        /**
         * Animates the pending {@link android.view.View} back to its original position.
         */
        private void restoreCurrentViewTranslation()
        {
            if (mCurrentView == null)
            {
                return;
            }

            ObjectAnimator xAnimator = ObjectAnimator.OfFloat(mSwipingView, TRANSLATION_X, 0);
            ObjectAnimator alphaAnimator = ObjectAnimator.OfFloat(mSwipingView, ALPHA, 1);

            AnimatorSet animatorSet = new AnimatorSet();
            animatorSet.PlayTogether(xAnimator, alphaAnimator);
            animatorSet.SetDuration(mAnimationTime);
            animatorSet.AddListener(new RestoreAnimatorListener(mCurrentView, mCurrentPosition,this));
            animatorSet.Start();
        }

        /**
         * Resets the fields to the initial values, ready to start over.
         */
        private void reset()
        {
            if (mVelocityTracker != null)
            {
                mVelocityTracker.Recycle();
            }

            mVelocityTracker = null;
            mDownX = 0;
            mDownY = 0;
            mCurrentView = null;
            mSwipingView = null;
            mCurrentPosition = AdapterView.InvalidPosition;
            mSwiping = false;
            mCanDismissCurrent = false;
        }

        /**
         * Called when the user starts swiping a {@link android.view.View}.
         *
         * @param view     the {@code View} that is being swiped.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         */
        protected void onStartSwipe(View view, int position)
        {
        }

        /**
         * Called when the swipe movement is canceled. A restore animation starts at this point.
         *
         * @param view     the {@code View} that was swiped.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         */
        protected void onCancelSwipe(View view, int position)
        {
        }

        /**
         * Called after the restore animation of a canceled swipe movement ends.
         *
         * @param view     the {@code View} that is being swiped.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         */
        protected virtual void afterCancelSwipe(View view, int position)
        {
        }

        /**
         * Called when the user lifted their finger off the screen, and the {@link android.view.View} should be swiped away. A fling animation starts at this point.
         *
         * @param view     the {@code View} that is being flinged.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         */
        protected void beforeViewFling(View view, int position)
        {
        }

        /**
         * Returns whether flinging the item at given position in the current state
         * would cause it to be removed from the data set.
         *
         * @param view the {@code View} that would be flinged.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         *
         * @return {@code true} if the item would leave the data set, {@code false} otherwise.
         */
        protected abstract bool willLeaveDataSetOnFling(View view, int position);

        /**
         * Called after the fling animation of a succesful swipe ends.
         * Users of this class should implement any finalizing behavior at this point, such as notifying the adapter.
         *
         * @param view     the {@code View} that is being swiped.
         * @param position the position of the item in the {@link android.widget.ListAdapter} corresponding to the {@code View}.
         */
        protected abstract void afterViewFling(View view, int position);

        /**
         * Restores the {@link android.view.View}'s {@code alpha} and {@code translationX} values.
         * Users of this class should call this method when recycling {@code View}s.
         *
         * @param view the {@code View} whose presentation should be restored.
         */
        protected virtual void restoreViewPresentation(View view)
        {
            View swipedView = getSwipeView(view);
            swipedView.Alpha = 1;
            //ViewHelper.setAlpha(swipedView, 1);
            //ViewHelper.setTranslationX(swipedView, 0);
            swipedView.TranslationX = 0;
        }

        /**
         * Returns the number of active swipe animations.
         */
        protected int getActiveSwipeCount()
        {
            return mActiveSwipeCount;
        }

        /**
         * Returns the {@link android.view.View} that should be swiped away. Must be a child of given {@code View}, or the {@code View} itself.
         *
         * @param view the parent {@link android.view.View}.
         */
        //@NonNull
        protected View getSwipeView(View view)
        {
            return view;
        }

        private static Rect getChildViewRect(View parentView, View childView)
        {
            Rect childRect = new Rect(childView.Left, childView.Top, childView.Right, childView.Bottom);
            if (!parentView.Equals(childView))
            {
                View workingChildView = childView;
                ViewGroup parent;
                while (!(parent = (ViewGroup)workingChildView.Parent).Equals(parentView))
                {
                    childRect.Offset(parent.Left, parent.Top);
                    workingChildView = parent;
                }
            }
            return childRect;
        }

        /**
         * An {@link com.nineoldandroids.animation.Animator.AnimatorListener} that notifies when the fling animation has ended.
         */
        private class FlingAnimatorListener : AnimatorListenerAdapter
        {

            //@NonNull
            private View mView;

            private int mPosition;
            private SwipeTouchListener minst;

            public  FlingAnimatorListener(View view, int position,SwipeTouchListener instance)
            {
                mView = view;
                mPosition = position;
                minst = instance;
            }

            //@Override
            public override void OnAnimationEnd(Animator animation)
            {
                minst.mActiveSwipeCount--;
                minst.afterViewFling(mView, mPosition);
            }
        }

        /**
         * An {@link com.nineoldandroids.animation.Animator.AnimatorListener} that performs the dismissal animation when the current animation has ended.
         */
        private class RestoreAnimatorListener : AnimatorListenerAdapter
        {

            //@NonNull
            private View mView;

            private int mPosition;
            private SwipeTouchListener minst;

            internal RestoreAnimatorListener(View view, int position,SwipeTouchListener instance)
            {
                mView = view;
                mPosition = position;
                minst = instance;
            }

            //@Override
            public void onAnimationEnd(Animator animation)
            {
                minst.mActiveSwipeCount--;
                minst.afterCancelSwipe(mView, mPosition);
            }
        }
    }
}