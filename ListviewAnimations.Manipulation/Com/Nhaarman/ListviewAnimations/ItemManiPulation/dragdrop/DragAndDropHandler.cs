//package com.nhaarman.listviewanimations.itemmanipulation.dragdrop;

//import android.animation.Animator;
//import android.animation.AnimatorListenerAdapter;
//import android.animation.ValueAnimator;
//import android.annotation.TargetApi;
//import android.content.res.Resources;
//import android.graphics.Canvas;
//import android.graphics.Rect;
//import android.os.Build;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.util.TypedValue;
//import android.view.MotionEvent;
//import android.view.View;
//import android.view.ViewConfiguration;
//import android.view.ViewTreeObserver;
//import android.widget.AbsListView;
//import android.widget.AdapterView;
//import android.widget.BaseAdapter;
//import android.widget.ListAdapter;
//import android.widget.WrapperListAdapter;

//import com.nhaarman.listviewanimations.itemmanipulation.DynamicListView;
//import com.nhaarman.listviewanimations.itemmanipulation.TouchEventHandler;
//import com.nhaarman.listviewanimations.util.Swappable;

using Android.Animation;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
/**
 * A class which handles drag and drop functionality for listview implementations backed up by a
 * {@link com.nhaarman.listviewanimations.util.Swappable} {@link ListAdapter}.
 * This class only works properly on API levels 14 and higher.
 * <p/>
 * Users of this class must call {@link #onTouchEvent(android.view.MotionEvent)} and {@link #dispatchDraw(android.graphics.Canvas)} on the right moments.
 */
//@TargetApi(14)
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{
    public class DragAndDropHandler : TouchEventHandler
    {

        private static readonly int INVALID_ID = -1;

        //@NonNull
        private DragAndDropListViewWrapper mWrapper;

        /**
         * The {@link ScrollHandler} that handles scrolling when dragging an item.
         */
        //@NonNull
        private ScrollHandler mScrollHandler;

        /**
         * The {@link SwitchViewAnimator} that is responsible for animating the switch views.
         */
        //@NonNull
        private SwitchViewAnimator mSwitchViewAnimator;

        /**
         * The minimum distance in pixels that should be moved before starting vertical item movement.
         */
        private int mSlop;

        /**
         * The {@link android.widget.ListAdapter} that is assigned. Also implements {@link com.nhaarman.listviewanimations.util.Swappable}.
         */
        //@Nullable
        private IListAdapter mAdapter;

        /**
         * The Drawable that is drawn when the user is dragging an item.
         * This value is null if and only if the user is not dragging.
         */
        //@Nullable
        private HoverDrawable mHoverDrawable;

        /**
         * The View that is represented by {@link #mHoverDrawable}.
         * When this value is not null, the View should be invisible.
         * This value is null if and only if the user is not dragging.
         */
        //@Nullable
        private View mMobileView;

        /**
         * The id of the item view that is being dragged.
         * This value is {@value #INVALID_ID} if and only if the user is not dragging.
         */
        private long mMobileItemId;

        /**
         * The y coordinate of the last non-final {@code MotionEvent}.
         */
        private float mLastMotionEventY = -1;

        /**
         * The original position of the view that is being dragged.
         * This value is {@value android.widget.AdapterView#INVALID_POSITION} if and only if the user is not dragging.
         */
        private int mOriginalMobileItemPosition = AdapterView.InvalidPosition;

        /**
         * The {@link DraggableManager} responsible for deciding if an item can be dragged.
         */
        //@NonNull
        private DraggableManager mDraggableManager;

        /**
         * The {@link OnItemMovedListener} that is notified of moved items.
         */
        //@Nullable
        private OnItemMovedListener mOnItemMovedListener;

        /**
         * The raw x coordinate of the down event.
         */
        private float mDownX;

        /**
         * The raw y coordinate of the down event.
         */
        private float mDownY;

        /**
         * Specifies whether or not the hover drawable is currently being animated as result of an up / cancel event.
         */
        private bool mIsSettlingHoverDrawable;

        /**
         * Creates a new {@code DragAndDropHandler} for given {@link com.nhaarman.listviewanimations.itemmanipulation.DynamicListView}.
         *
         * @param dynamicListView the {@code DynamicListView} to use.
         */
        public DragAndDropHandler(DynamicListView dynamicListView)
            : this(new DynamicListViewWrapper(dynamicListView))
        {

        }

        /**
         * Creates a new {@code DragAndDropHandler} for the listview implementation
         * in given {@link com.nhaarman.listviewanimations.itemmanipulation.dragdrop.DragAndDropListViewWrapper}
         *
         * @param dragAndDropListViewWrapper the {@code DragAndDropListViewWrapper} which wraps the listview implementation to use.
         */
        public DragAndDropHandler(DragAndDropListViewWrapper dragAndDropListViewWrapper)
        {
            mWrapper = dragAndDropListViewWrapper;
            if (mWrapper.getAdapter() != null)
            {
                setAdapterInternal(mWrapper.getAdapter());
            }

            mScrollHandler = new ScrollHandler(this);
            mWrapper.setOnScrollListener(mScrollHandler);

            mDraggableManager = new DefaultDraggableManager();

            if (Build.VERSION.SdkInt <= BuildVersionCodes.Kitkat)
            {
                mSwitchViewAnimator = new KitKatSwitchViewAnimator(this);
            }
            else
            {
                mSwitchViewAnimator = new LSwitchViewAnimator(this);
            }

            mMobileItemId = INVALID_ID;

            ViewConfiguration vc = ViewConfiguration.Get(dragAndDropListViewWrapper.getListView().Context);
            mSlop = vc.ScaledTouchSlop;
        }


        /**
         * @throws java.lang.IllegalStateException    if the adapter does not have stable ids.
         * @throws java.lang.IllegalArgumentException if the adapter does not implement {@link com.nhaarman.listviewanimations.util.Swappable}.
         */
        public void setAdapter(IListAdapter adapter)
        {
            setAdapterInternal(adapter);
        }

        /**
         * @throws java.lang.IllegalStateException    if the adapter does not have stable ids.
         * @throws java.lang.IllegalArgumentException if the adapter does not implement {@link com.nhaarman.listviewanimations.util.Swappable}.
         */
        private void setAdapterInternal(IListAdapter adapter)
        {
            IListAdapter actualAdapter = adapter;
            if (actualAdapter is IWrapperListAdapter)
            {
                actualAdapter = ((IWrapperListAdapter)actualAdapter).WrappedAdapter;
            }

            if (!actualAdapter.HasStableIds)
            {
                throw new Java.Lang.IllegalStateException("Adapter doesn't have stable ids! Make sure your adapter has stable ids, and override hasStableIds() to return true.");
            }

            if (!(actualAdapter is Swappable))
            {
                throw new Java.Lang.IllegalArgumentException("Adapter should implement Swappable!");
            }

            mAdapter = actualAdapter;
        }

        /**
         * Sets the scroll speed when dragging an item. Defaults to {@code 1.0f}.
         *
         * @param speed {@code <1.0f} to slow down scrolling, {@code >1.0f} to speed up scrolling.
         */
        public void setScrollSpeed(float speed)
        {
            mScrollHandler.setScrollSpeed(speed);
        }

        /**
         * Starts dragging the item at given position. User must be touching this {@code DynamicListView}.
         *
         * @param position the position of the item in the adapter to start dragging. Be sure to subtract any header views.
         *
         * @throws java.lang.IllegalStateException if the user is not touching this {@code DynamicListView},
         *                                         or if there is no adapter set.
         */
        public void startDragging(int position)
        {
            if (mMobileItemId != INVALID_ID)
            {
                /* We are already dragging */
                return;
            }

            if (mLastMotionEventY < 0)
            {
                throw new Java.Lang.IllegalStateException("User must be touching the DynamicListView!");
            }

            if (mAdapter == null)
            {
                throw new Java.Lang.IllegalStateException("This DynamicListView has no adapter set!");
            }

            if (position < 0 || position >= mAdapter.Count)
            {
                /* Out of bounds */
                return;
            }


            mMobileView = mWrapper.getChildAt(position - mWrapper.getFirstVisiblePosition() + mWrapper.getHeaderViewsCount());
            if (mMobileView != null)
            {
                mOriginalMobileItemPosition = position;
                mMobileItemId = mAdapter.GetItemId(position);
                mHoverDrawable = new HoverDrawable(mMobileView, mLastMotionEventY);
                mMobileView.Visibility = ViewStates.Invisible;
            }
        }

        /**
         * Sets the {@link DraggableManager} to be used for determining whether an item should be dragged when the user issues a down {@code MotionEvent}.
         */
        public void setDraggableManager(DraggableManager draggableManager)
        {
            mDraggableManager = draggableManager;
        }

        /**
         * Sets the {@link com.nhaarman.listviewanimations.itemmanipulation.dragdrop.OnItemMovedListener} that is notified when user has dropped a dragging item.
         */
        public void setOnItemMovedListener(OnItemMovedListener onItemMovedListener)
        {
            mOnItemMovedListener = onItemMovedListener;
        }

        //@Override
        public bool isInteracting()
        {
            return mMobileItemId != INVALID_ID;
        }

        /**
         * Dispatches the {@link android.view.MotionEvent}s to their proper methods if applicable.
         *
         * @param event the {@code MotionEvent}.
         *
         * @return {@code true} if the event was handled, {@code false} otherwise.
         */
        // @Override
        public  bool onTouchEvent(MotionEvent mevent)
        {
            bool handled = false;

            /* We are in the process of animating the hover drawable back, do not start a new drag yet. */
            if (!mIsSettlingHoverDrawable)
            {
                switch (mevent.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        mLastMotionEventY = mevent.GetY();
                        handled = handleDownEvent(mevent);
                        break;
                    case MotionEventActions.Move:
                        mLastMotionEventY = mevent.GetY();
                        handled = handleMoveEvent(mevent);
                        break;
                    case MotionEventActions.Up:
                        handled = handleUpEvent();
                        mLastMotionEventY = -1;
                        break;
                    case MotionEventActions.Cancel:
                        handled = handleCancelEvent();
                        mLastMotionEventY = -1;
                        break;
                    default:
                        handled = false;
                        break;
                }
            }
            return handled;
        }

        /**
         * Handles the down event.
         * <p/>
         * Finds the position and {@code View} of the touch point and, if allowed by the {@link com.nhaarman.listviewanimations.itemmanipulation.dragdrop.DraggableManager},
         * starts dragging the {@code View}.
         *
         * @param event the {@link android.view.MotionEvent} that was triggered.
         *
         * @return {@code true} if we have started dragging, {@code false} otherwise.
         */
        private bool handleDownEvent(MotionEvent mevent)
        {
            mDownX = mevent.RawX;
            mDownY = mevent.RawY;
            return true;
        }

        /**
         * Retrieves the position in the list corresponding to itemId.
         *
         * @return the position of the item in the list, or {@link android.widget.AdapterView#INVALID_POSITION} if the {@code View} corresponding to the id was not found.
         */
        private int getPositionForId(long itemId)
        {
            View v = getViewForId(itemId);
            if (v == null)
            {
                return AdapterView.InvalidPosition;
            }
            else
            {
                return mWrapper.getPositionForView(v);
            }
        }

        /**
         * Retrieves the {@code View} in the list corresponding to itemId.
         *
         * @return the {@code View}, or {@code null} if not found.
         */
        //@Nullable
        private View getViewForId(long itemId)
        {
            IListAdapter adapter = mAdapter;
            if (itemId == INVALID_ID || adapter == null)
            {
                return null;
            }

            int firstVisiblePosition = mWrapper.getFirstVisiblePosition();

            View result = null;
            for (int i = 0; i < mWrapper.getChildCount() && result == null; i++)
            {
                int position = firstVisiblePosition + i;
                if (position - mWrapper.getHeaderViewsCount() >= 0)
                {
                    long id = adapter.GetItemId(position - mWrapper.getHeaderViewsCount());
                    if (id == itemId)
                    {
                        result = mWrapper.getChildAt(i);
                    }
                }
            }
            return result;
        }

        /**
         * Handles the move events.
         * <p/>
         * Applies the {@link MotionEvent} to the hover drawable, and switches {@code View}s if necessary.
         *
         * @param event the {@code MotionEvent}.
         *
         * @return {@code true} if the event was handled, {@code false} otherwise.
         */
        private bool handleMoveEvent(MotionEvent mevent)
        {
            bool handled = false;

            float deltaX = mevent.RawX - mDownX;
            float deltaY = mevent.RawY - mDownY;

            if (mHoverDrawable == null && Math.Abs(deltaY) > mSlop && Math.Abs(deltaY) > Math.Abs(deltaX))
            {
                int position = mWrapper.pointToPosition((int)mevent.GetX(), (int)mevent.GetY());
                if (position != AdapterView.InvalidPosition)
                {
                    View downView = mWrapper.getChildAt(position - mWrapper.getFirstVisiblePosition());
                    //assert downView != null;
                    if (mDraggableManager.isDraggable(downView, position - mWrapper.getHeaderViewsCount(), mevent.GetX() - downView.GetX(), mevent.GetY() - downView.GetY()))
                    {
                        startDragging(position - mWrapper.getHeaderViewsCount());
                        handled = true;
                    }
                }
            }
            else if (mHoverDrawable != null)
            {
                mHoverDrawable.handleMoveEvent(mevent);

                switchIfNecessary();
                mWrapper.getListView().Invalidate();
                handled = true;
            }

            return handled;
        }

        /**
         * Finds the {@code View} that is a candidate for switching, and executes the switch if necessary.
         */
        private void switchIfNecessary()
        {
            if (mHoverDrawable == null || mAdapter == null)
            {
                return;
            }

            int position = getPositionForId(mMobileItemId);
            long aboveItemId = position - 1 - mWrapper.getHeaderViewsCount() >= 0 ? mAdapter.GetItemId(position - 1 - mWrapper.getHeaderViewsCount()) : INVALID_ID;
            long belowItemId = position + 1 - mWrapper.getHeaderViewsCount() < mAdapter.Count
                               ? mAdapter.GetItemId(position + 1 - mWrapper.getHeaderViewsCount())
                               : INVALID_ID;

            long switchId = mHoverDrawable.isMovingUpwards() ? aboveItemId : belowItemId;
            View switchView = getViewForId(switchId);

            int deltaY = mHoverDrawable.getDeltaY();
            if (switchView != null && Math.Abs(deltaY) > mHoverDrawable.IntrinsicHeight)
            {
                switchViews(switchView, switchId, mHoverDrawable.IntrinsicHeight * (deltaY < 0 ? -1 : 1));
            }

            mScrollHandler.handleMobileCellScroll();

            mWrapper.getListView().Invalidate();
        }

        /**
         * Switches the item that is currently being dragged with the item belonging to given id,
         * by notifying the adapter to swap positions and that the data set has changed.
         *
         * @param switchView   the {@code View} that should be animated towards the old position of the currently dragging item.
         * @param switchId     the id of the item that will take the position of the currently dragging item.
         * @param translationY the distance in pixels the {@code switchView} should animate - i.e. the (positive or negative) height of the {@code View} corresponding to the currently
         *                     dragging item.
         */
        private void switchViews(View switchView, long switchId, float translationY)
        {
            //assert mHoverDrawable != null;
            //assert mAdapter != null;
            //assert mMobileView != null;

            int switchViewPosition = mWrapper.getPositionForView(switchView);
            int mobileViewPosition = mWrapper.getPositionForView(mMobileView);

            ((Swappable)mAdapter).swapItems(switchViewPosition - mWrapper.getHeaderViewsCount(), mobileViewPosition - mWrapper.getHeaderViewsCount());
            ((BaseAdapter)mAdapter).NotifyDataSetChanged();

            mHoverDrawable.shift(switchView.Height);
            mSwitchViewAnimator.animateSwitchView(switchId, translationY);
        }

        /**
         * Handles the up event.
         * <p/>
         * Animates the hover drawable to its final position, and finalizes our drag properties when the animation has finished.
         * Will also notify the {@link com.nhaarman.listviewanimations.itemmanipulation.dragdrop.OnItemMovedListener} set if applicable.
         *
         * @return {@code true} if the event was handled, {@code false} otherwise.
         */
        private bool handleUpEvent()
        {
            if (mMobileView == null)
            {
                return false;
            }
            //assert mHoverDrawable != null;

            ValueAnimator valueAnimator = ValueAnimator.OfInt(mHoverDrawable.getTop(), (int)mMobileView.GetY());
            SettleHoverDrawableAnimatorListener listener = new SettleHoverDrawableAnimatorListener(mHoverDrawable, mMobileView,this);
            valueAnimator.AddUpdateListener(listener);
            valueAnimator.AddListener(listener);
            valueAnimator.Start();

            int newPosition = getPositionForId(mMobileItemId) - mWrapper.getHeaderViewsCount();
            if (mOriginalMobileItemPosition != newPosition && mOnItemMovedListener != null)
            {
                mOnItemMovedListener.onItemMoved(mOriginalMobileItemPosition, newPosition);
            }

            return true;
        }

        /**
         * Handles the cancel event.
         *
         * @return {@code true} if the event was handled, {@code false} otherwise.
         */
        private bool handleCancelEvent()
        {
            return handleUpEvent();
        }

        public void dispatchDraw(Canvas canvas)
        {
            if (mHoverDrawable != null)
            {
                mHoverDrawable.Draw(canvas);
            }
        }

        /**
         * An interface for animating the switch views.
         * A distinction is made between API levels because {@link android.widget.AbsListView.OnScrollListener#onScroll(android.widget.AbsListView, int, int,
         * int)} calling behavior differs.
         */
        private interface SwitchViewAnimator
        {

            void animateSwitchView(long switchId, float translationY);
        }

        /**
         * By default, nothing is draggable. User should set a {@link com.nhaarman.listviewanimations.itemmanipulation.dragdrop.DraggableManager} manually,
         * or use {@link #startDragging(int)} if they want to start a drag (for example using a long click listener).
         */
        private class DefaultDraggableManager : DraggableManager
        {

            //@Override
            public bool isDraggable(View view, int position, float x, float y)
            {
                return false;
            }
        }

        /**
         * A {@link SwitchViewAnimator} for versions KitKat and below.
         * This class immediately updates {@link #mMobileView} to be the newly mobile view.
         */
        private class KitKatSwitchViewAnimator : SwitchViewAnimator
        {
            private DragAndDropHandler minst;

            public KitKatSwitchViewAnimator(DragAndDropHandler instance)
            {
                minst = instance;
 
            }

            //@Override
            public void animateSwitchView(long switchId, float translationY)
            {
                //assert mMobileView != null;
               
                minst.mWrapper.getListView().ViewTreeObserver.AddOnPreDrawListener(new AnimateSwitchViewOnPreDrawListener(minst.mMobileView, switchId, translationY,minst));
                minst.mMobileView = minst.getViewForId(minst.mMobileItemId);
            }

            private class AnimateSwitchViewOnPreDrawListener :Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
            {

                private View mPreviousMobileView;

                private long mSwitchId;

                private float mTranslationY;
                private DragAndDropHandler nestinst;

                public AnimateSwitchViewOnPreDrawListener(View previousMobileView, long switchId, float translationY, DragAndDropHandler instance)
                {
                    mPreviousMobileView = previousMobileView;
                    mSwitchId = switchId;
                    mTranslationY = translationY;
                    nestinst = instance;
                }

                //@Override
                public bool OnPreDraw()
                {
                    nestinst.mWrapper.getListView().ViewTreeObserver.RemoveOnPreDrawListener(this);

                    View switchView = nestinst.getViewForId(mSwitchId);
                    if (switchView != null)
                    {
                        switchView.TranslationY = mTranslationY;
                        switchView.Animate().TranslationY(0).Start();
                    }

                    mPreviousMobileView.Visibility = ViewStates.Visible;

                    if (nestinst.mMobileView != null)
                    {
                        nestinst.mMobileView.Visibility = ViewStates.Invisible;
                    }
                    return true;
                }
            }
        }

        /**
         * A {@link SwitchViewAnimator} for versions L and above.
         * This class updates {@link #mMobileView} only after the next frame has been drawn.
         */
        private class LSwitchViewAnimator : SwitchViewAnimator
        {

            private DragAndDropHandler inst;

            public LSwitchViewAnimator(DragAndDropHandler instance)
            {
                inst = instance;
            }

            //@Override
            public void animateSwitchView(long switchId, float translationY)
            {
                
                inst.mWrapper.getListView().ViewTreeObserver.AddOnPreDrawListener(new AnimateSwitchViewOnPreDrawListener(switchId, translationY, inst));
            }

            private class AnimateSwitchViewOnPreDrawListener :Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
            {

                private long mSwitchId;

                private float mTranslationY;

                private DragAndDropHandler inst2;

                internal AnimateSwitchViewOnPreDrawListener(long switchId, float translationY, DragAndDropHandler instance)
                {
                    mSwitchId = switchId;
                    mTranslationY = translationY;
                    inst2 = instance;
                }

                //@Override
                public bool OnPreDraw()
                {
                    inst2.mWrapper.getListView().ViewTreeObserver.RemoveOnPreDrawListener(this);

                    View switchView = inst2.getViewForId(mSwitchId);
                    if (switchView != null)
                    {
                        switchView.TranslationY = mTranslationY;
                        switchView.Animate().TranslationY(0).Start();
                    }

                    //assert mMobileView != null;
                    inst2.mMobileView.Visibility = ViewStates.Visible;
                    inst2.mMobileView = inst2.getViewForId(inst2.mMobileItemId);
                    //assert mMobileView != null;
                    inst2.mMobileView.Visibility = ViewStates.Invisible;
                    return true;
                }
            }
        }


        /**
         * A class which handles scrolling for this {@code DynamicListView} when dragging an item.
         * <p/>
         * The {@link #handleMobileCellScroll()} method initiates the scroll and should typically be called on a move {@code MotionEvent}.
         * <p/>
         * The {@link #onScroll(android.widget.AbsListView, int, int, int)} method then takes over the functionality {@link #handleMoveEvent(android.view.MotionEvent)} provides.
         */
        class ScrollHandler :Java.Lang.Object, AbsListView.IOnScrollListener
        {

            private static readonly int SMOOTH_SCROLL_DP = 3;

            /**
             * The default scroll amount in pixels.
             */
            private int mSmoothScrollPx;

            /**
             * The factor to multiply {@link #mSmoothScrollPx} with for scrolling.
             */
            private float mScrollSpeedFactor = 1.0f;

            /**
             * The previous first visible item before checking if we should switch.
             */
            private int mPreviousFirstVisibleItem = -1;

            /**
             * The previous last visible item before checking if we should switch.
             */
            private int mPreviousLastVisibleItem = -1;

            /**
             * The current first visible item.
             */
            private int mCurrentFirstVisibleItem;

            /**
             * The current last visible item.
             */
            private int mCurrentLastVisibleItem;


            private DragAndDropHandler minst;

            public ScrollHandler(DragAndDropHandler instance)
            {
                minst = instance;
                Resources r = minst.mWrapper.getListView().Resources;
                
                mSmoothScrollPx = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip  , SMOOTH_SCROLL_DP, r.DisplayMetrics);
            }

            /**
             * Sets the scroll speed when dragging an item. Defaults to {@code 1.0f}.
             *
             * @param scrollSpeedFactor {@code <1.0f} to slow down scrolling, {@code >1.0f} to speed up scrolling.
             */
            internal void setScrollSpeed(float scrollSpeedFactor)
            {
                mScrollSpeedFactor = scrollSpeedFactor;
            }

            /**
             * Scrolls the {@code DynamicListView} if the hover drawable is above or below the bounds of the {@code ListView}.
             */
            internal void handleMobileCellScroll()
            {
                if (minst.mHoverDrawable == null || minst.mIsSettlingHoverDrawable)
                {
                    return;
                }

                Rect r = minst.mHoverDrawable.Bounds;
                int offset = minst.mWrapper.computeVerticalScrollOffset();
                int height = minst.mWrapper.getListView().Height;
                int extent = minst.mWrapper.computeVerticalScrollExtent();
                int range = minst.mWrapper.computeVerticalScrollRange();
                int hoverViewTop = r.Top;
                int hoverHeight = r.Height();

                int scrollPx = (int)Math.Max(1, mSmoothScrollPx * mScrollSpeedFactor);
                if (hoverViewTop <= 0 && offset > 0)
                {
                    minst.mWrapper.smoothScrollBy(-scrollPx, 0);
                }
                else if (hoverViewTop + hoverHeight >= height && offset + extent < range)
                {
                    minst.mWrapper.smoothScrollBy(scrollPx, 0);
                }
            }

            // @Override
            public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
            {
                mCurrentFirstVisibleItem = firstVisibleItem;
                mCurrentLastVisibleItem = firstVisibleItem + visibleItemCount;

                mPreviousFirstVisibleItem = mPreviousFirstVisibleItem == -1 ? mCurrentFirstVisibleItem : mPreviousFirstVisibleItem;
                mPreviousLastVisibleItem = mPreviousLastVisibleItem == -1 ? mCurrentLastVisibleItem : mPreviousLastVisibleItem;

                if (minst.mHoverDrawable != null)
                {
                    //assert mMobileView != null;
                    float y = minst.mMobileView.GetY();
                    minst.mHoverDrawable.onScroll(y);
                }

                if (!minst.mIsSettlingHoverDrawable)
                {
                    checkAndHandleFirstVisibleCellChange();
                    checkAndHandleLastVisibleCellChange();
                }

                mPreviousFirstVisibleItem = mCurrentFirstVisibleItem;
                mPreviousLastVisibleItem = mCurrentLastVisibleItem;
            }

            //@Override
            public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
            {
                if (scrollState == ScrollState.Idle && minst.mHoverDrawable != null)
                {
                    handleMobileCellScroll();
                }
            }

            /**
             * Determines if the listview scrolled up enough to reveal a new cell at the
             * top of the list. If so, switches the newly shown view with the mobile view.
             */
            private void checkAndHandleFirstVisibleCellChange()
            {
                if (minst.mHoverDrawable == null || minst.mAdapter == null || mCurrentFirstVisibleItem >= mPreviousFirstVisibleItem)
                {
                    return;
                }

                int position = minst.getPositionForId(minst.mMobileItemId);
                if (position == AdapterView.InvalidPosition)
                {
                    return;
                }

                long switchItemId = position - 1 - minst.mWrapper.getHeaderViewsCount() >= 0 ? minst.mAdapter.GetItemId(position - 1 - minst.mWrapper.getHeaderViewsCount()) : INVALID_ID;
                View switchView = minst.getViewForId(switchItemId);
                if (switchView != null)
                {
                    minst.switchViews(switchView, switchItemId, -switchView.Height);
                }
            }

            /**
             * Determines if the listview scrolled down enough to reveal a new cell at the
             * bottom of the list. If so, switches the newly shown view with the mobile view.
             */
            private void checkAndHandleLastVisibleCellChange()
            {
                if (minst.mHoverDrawable == null || minst.mAdapter == null || mCurrentLastVisibleItem <= mPreviousLastVisibleItem)
                {
                    return;
                }

                int position = minst.getPositionForId(minst.mMobileItemId);
                if (position == AdapterView.InvalidPosition)
                {
                    return;
                }

                long switchItemId = position + 1 - minst.mWrapper.getHeaderViewsCount() < minst.mAdapter.Count
                                    ? minst.mAdapter.GetItemId(position + 1 - minst.mWrapper.getHeaderViewsCount())
                                    : INVALID_ID;
                View switchView = minst.getViewForId(switchItemId);
                if (switchView != null)
                {
                    minst.switchViews(switchView, switchItemId, switchView.Height);
                }
            }
        }

        /**
         * Updates the hover drawable's bounds with the animated values.
         * When the animation has finished, it will reset all the drag properties.
         */
        private class SettleHoverDrawableAnimatorListener : AnimatorListenerAdapter, ValueAnimator.IAnimatorUpdateListener
        {

            //@NonNull
            private HoverDrawable mAnimatingHoverDrawable;

            //@NonNull
            private View mAnimatingMobileView;

            private DragAndDropHandler minst;

            public SettleHoverDrawableAnimatorListener(HoverDrawable animatingHoverDrawable, View animatingMobileView, DragAndDropHandler instance)
            {
                mAnimatingHoverDrawable = animatingHoverDrawable;
                mAnimatingMobileView = animatingMobileView;
                minst = instance;
            }

            //@Override
            public override void OnAnimationStart(Animator animation)
            {
                minst.mIsSettlingHoverDrawable = true;
            }

            //@Override
            public  void OnAnimationUpdate(ValueAnimator animation)
            {
                mAnimatingHoverDrawable.setTop((int)animation.AnimatedValue);
                minst.mWrapper.getListView().PostInvalidate();
            }

            //@Override
            public override void OnAnimationEnd(Animator animation)
            {
                
                mAnimatingMobileView.Visibility = ViewStates.Visible;

                minst.mHoverDrawable = null;
                minst.mMobileView = null;
                minst.mMobileItemId = INVALID_ID;
                minst.mOriginalMobileItemPosition = AdapterView.InvalidPosition;

                minst.mIsSettlingHoverDrawable = false;
            }
        }
    }
}
