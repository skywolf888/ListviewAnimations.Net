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
//package com.nhaarman.listviewanimations.itemmanipulation.expandablelistitem;

//import android.content.Context;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.view.ViewGroup.LayoutParams;
//import android.widget.FrameLayout;
//import android.widget.LinearLayout;

//import com.nhaarman.listviewanimations.ArrayAdapter;
//import com.nhaarman.listviewanimations.util.AdapterViewUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nhaarman.listviewanimations.util.ListViewWrapperSetter;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.AnimatorListenerAdapter;
//import com.nineoldandroids.animation.ValueAnimator;

//import java.util.ArrayList;
//import java.util.Collection;
//import java.util.HashSet;
//import java.util.List;

/**
 * An {@link ArrayAdapter} which allows items to be expanded using an animation.
 */
//@SuppressWarnings("UnusedDeclaration")

using Android.Animation;
using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;
using System;
using System.Collections.Generic;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.expandablelistitem
{
    public abstract class ExpandableListItemAdapter<T> : ArrayAdapter<T>, ListViewWrapperSetter
    {

        private static readonly int DEFAULTTITLEPARENTRESID = 10000;
        private static readonly int DEFAULTCONTENTPARENTRESID = 10001;

        //@NonNull
        private Context mContext;
        private int mTitleParentResId;
        private int mContentParentResId;

        //@NonNull
        private List<long> mExpandedIds;
        private int mViewLayoutResId;
        private int mActionViewResId;
        private int mLimit;

        //@Nullable
        private ListViewWrapper mListViewWrapper;

        //@Nullable
        private ExpandCollapseListener mExpandCollapseListener;

        /**
         * Creates a new ExpandableListItemAdapter with an empty list.
         */
        protected ExpandableListItemAdapter(Context context)
            : this(context, null)
        {
            //this(context, null);
        }

        /**
         * Creates a new {@code ExpandableListItemAdapter} with the specified list,
         * or an empty list if items == null.
         */
        protected ExpandableListItemAdapter(Context context, List<T> items)
            : base(items)
        {
            //super(items);
            mContext = context;
            mTitleParentResId = DEFAULTTITLEPARENTRESID;
            mContentParentResId = DEFAULTCONTENTPARENTRESID;

            mExpandedIds = new List<long>();
        }

        /**
         * Creates a new ExpandableListItemAdapter with an empty list. Uses given
         * layout resource for the view; titleParentResId and contentParentResId
         * should be identifiers for ViewGroups within that layout.
         */
        protected ExpandableListItemAdapter(Context context, int layoutResId, int titleParentResId, int contentParentResId)
            : this(context, layoutResId, titleParentResId, contentParentResId, null)
        {
            //this(context, layoutResId, titleParentResId, contentParentResId, null);
        }

        /**
         * Creates a new ExpandableListItemAdapter with the specified list, or an
         * empty list if items == null. Uses given layout resource for the view;
         * titleParentResId and contentParentResId should be identifiers for
         * ViewGroups within that layout.
         */
        protected ExpandableListItemAdapter(Context context, int layoutResId, int titleParentResId, int contentParentResId,
                                              List<T> items)
            : base(items)
        {
            //super(items);
            mContext = context;
            mViewLayoutResId = layoutResId;
            mTitleParentResId = titleParentResId;
            mContentParentResId = contentParentResId;

            mExpandedIds = new List<long>();
        }

        //@SuppressWarnings("NullableProblems")
        //@Override
        public   void setListViewWrapper(ListViewWrapper listViewWrapper)
        {
            mListViewWrapper = listViewWrapper;
        }

        /**
         * Set the resource id of the child {@link android.view.View} contained in the View
         * returned by {@link #getTitleView(int, android.view.View, android.view.ViewGroup)} that will be the
         * actuator of the expand / collapse animations.<br>
         * If there is no View in the title View with given resId, a
         * {@link NullPointerException} is thrown.</p> Default behavior: the whole
         * title View acts as the actuator.
         *
         * @param resId the resource id.
         */
        public void setActionViewResId(int resId)
        {
            mActionViewResId = resId;
        }

        /**
         * Set the maximum number of items allowed to be expanded. When the
         * (limit+1)th item is expanded, the first expanded item will collapse.
         *
         * @param limit the maximum number of items allowed to be expanded. Use <= 0
         *              for no limit.
         */
        public void setLimit(int limit)
        {
            mLimit = limit;
            mExpandedIds.Clear();
            notifyDataSetChanged();
        }

        /**
         * Set the {@link ExpandCollapseListener} that should be notified of expand / collapse events.
         */
        public void setExpandCollapseListener(ExpandCollapseListener expandCollapseListener)
        {
            mExpandCollapseListener = expandCollapseListener;
        }

        //@Override
        //@NonNull
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewGroup view = (ViewGroup)convertView;
            ViewHolder viewHolder;

            if (view == null)
            {
                view = createView(parent);

                viewHolder = new ViewHolder();
                viewHolder.titleParent = (ViewGroup)view.FindViewById(mTitleParentResId);
                viewHolder.contentParent = (ViewGroup)view.FindViewById(mContentParentResId);

                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
            }

            View titleView = getTitleView(position, viewHolder.titleView, viewHolder.titleParent);
            if (!titleView.Equals(viewHolder.titleView))
            {
                viewHolder.titleParent.RemoveAllViews();
                viewHolder.titleParent.AddView(titleView);

                if (mActionViewResId == 0)
                {
                    
                    view.SetOnClickListener(new TitleViewOnClickListener(viewHolder.contentParent,this));
                }
                else
                {
                    view.FindViewById(mActionViewResId).SetOnClickListener(new TitleViewOnClickListener(viewHolder.contentParent,this));
                }
            }
            viewHolder.titleView = titleView;

            View contentView = getContentView(position, viewHolder.contentView, viewHolder.contentParent);
            if (!contentView.Equals(viewHolder.contentView))
            {
                viewHolder.contentParent.RemoveAllViews();
                viewHolder.contentParent.AddView(contentView);
            }
            viewHolder.contentView = contentView;

            viewHolder.contentParent.Visibility = mExpandedIds.Contains(GetItemId(position)) ? ViewStates.Visible : ViewStates.Gone;
            viewHolder.contentParent.Tag = GetItemId(position);

            ViewGroup.LayoutParams layoutParams = viewHolder.contentParent.LayoutParameters;
            layoutParams.Height = ViewGroup.LayoutParams.WrapContent;
            viewHolder.contentParent.LayoutParameters = layoutParams;

            return view;
        }

        /**
         * Get a View that displays the <b>title of the data</b> at the specified
         * position in the data set. You can either create a View manually or
         * inflate it from an XML layout file. When the View is inflated, the parent
         * View (GridView, ListView...) will apply default layout parameters unless
         * you use
         * {@link android.view.LayoutInflater#inflate(int, android.view.ViewGroup, boolean)}
         * to specify a root view and to prevent attachment to the root.
         *
         * @param position    The position of the item within the adapter's data set of the
         *                    item whose view we want.
         * @param convertView The old view to reuse, if possible. Note: You should check
         *                    that this view is non-null and of an appropriate type before
         *                    using. If it is not possible to convert this view to display
         *                    the correct data, this method can create a new view.
         * @param parent      The parent that this view will eventually be attached to
         *
         * @return A View corresponding to the title of the data at the specified
         * position.
         */
        //@NonNull
        public abstract View getTitleView(int position, View convertView, ViewGroup parent);

        /**
         * Get a View that displays the <b>content of the data</b> at the specified
         * position in the data set. You can either create a View manually or
         * inflate it from an XML layout file. When the View is inflated, the parent
         * View (GridView, ListView...) will apply default layout parameters unless
         * you use
         * {@link android.view.LayoutInflater#inflate(int, android.view.ViewGroup, boolean)}
         * to specify a root view and to prevent attachment to the root.
         *
         * @param position    The position of the item within the adapter's data set of the
         *                    item whose view we want.
         * @param convertView The old view to reuse, if possible. Note: You should check
         *                    that this view is non-null and of an appropriate type before
         *                    using. If it is not possible to convert this view to display
         *                    the correct data, this method can create a new view.
         * @param parent      The parent that this view will eventually be attached to
         *
         * @return A View corresponding to the content of the data at the specified
         * position.
         */
        //@NonNull
        public abstract View getContentView(int position, View convertView, ViewGroup parent);

        /**
         * Indicates if the item at the specified position is expanded.
         *
         * @param position Index of the view whose state we want.
         *
         * @return true if the view is expanded, false otherwise.
         */
        public bool isExpanded(int position)
        {
            long itemId = GetItemId(position);
            return mExpandedIds.Contains(itemId);
        }

        /**
         * Return the title view at the specified position.
         *
         * @param position Index of the view we want.
         *
         * @return the view if it exist, null otherwise.
         */
        //@Nullable
        public View getTitleView(int position)
        {
            View titleView = null;

            View parentView = findViewForPosition(position);
            if (parentView != null)
            {
                Object tag = parentView.Tag;
                if (tag is ViewHolder)
                {
                    titleView = ((ViewHolder)tag).titleView;
                }
            }

            return titleView;
        }

        /**
         * Return the content view at the specified position.
         *
         * @param position Index of the view we want.
         *
         * @return the view if it exist, null otherwise.
         */
        //@Nullable
        public View getContentView(int position)
        {
            View contentView = null;

            View parentView = findViewForPosition(position);
            if (parentView != null)
            {
                Object tag = parentView.Tag;
                if (tag is ViewHolder)
                {
                    contentView = ((ViewHolder)tag).contentView;
                }
            }

            return contentView;
        }

        //@Override
        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();

            ICollection<long> removedIds = new HashSet<long>(mExpandedIds);

            for (int i = 0; i < Count; ++i)
            {
                long id = GetItemId(i);
                removedIds.Remove(id);
            }
            mExpandedIds.RemoveAll(delegate(long item)
            {
                return removedIds.Contains(item);
            });
            //mExpandedIds.RemoveAll(removedIds);

        }

        

        /**
         * Expand the view at given position. Will do nothing if the view is already expanded.
         *
         * @param position the position to expand.
         */
        public void expand(int position)
        {
            long itemId = GetItemId(position);
            if (mExpandedIds.Contains(itemId))
            {
                return;
            }

            toggle(position);
        }

        /**
         * Collapse the view at given position. Will do nothing if the view is already collapsed.
         *
         * @param position the position to collapse.
         */
        public void collapse(int position)
        {
            long itemId = GetItemId(position);
            if (!mExpandedIds.Contains(itemId))
            {
                return;
            }

            toggle(position);
        }

        /**
         * Toggle the {@link android.view.View} at given position, ignores header or footer Views.
         *
         * @param position the position of the view to toggle.
         */
        public void toggle(int position)
        {
            long itemId = GetItemId(position);
            bool isExpanded = mExpandedIds.Contains(itemId);

            View contentParent = getContentParent(position);
            if (contentParent != null)
            {
                toggle(contentParent);
            }

            if (contentParent == null && isExpanded)
            {
                mExpandedIds.Remove(itemId);
            }
            else if (contentParent == null)
            {
                mExpandedIds.Add(itemId);
            }
        }

        //@NonNull
        private ViewGroup createView(ViewGroup parent)
        {
            ViewGroup view;

            if (mViewLayoutResId == 0)
            {
                view = new RootView(mContext);
            }
            else
            {
                view = (ViewGroup)LayoutInflater.From(mContext).Inflate(mViewLayoutResId, parent, false);
            }

            return view;
        }

        /**
         * Return the content parent at the specified position.
         *
         * @param position Index of the view we want.
         *
         * @return the view if it exist, null otherwise.
         */
        //@Nullable
        private View getContentParent(int position)
        {
            View contentParent = null;

            View parentView = findViewForPosition(position);
            if (parentView != null)
            {
                Java.Lang.Object tag = parentView.Tag;
                if (tag is ViewHolder)
                {
                    contentParent = ((ViewHolder)tag).contentParent;
                }
            }

            return contentParent;
        }

        //@Nullable
        private View findViewForPosition(int position)
        {
            if (mListViewWrapper == null)
            {
                throw new Java.Lang.IllegalStateException("Call setAbsListView on this ExpanableListItemAdapter!");
            }

            View result = null;
            for (int i = 0; i < mListViewWrapper.getChildCount() && result == null; i++)
            {
                View childView = mListViewWrapper.getChildAt(i);
                if (childView != null && AdapterViewUtil.getPositionForView(mListViewWrapper, childView) == position)
                {
                    result = childView;
                }
            }
            return result;
        }

        private int findPositionForId(long id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (GetItemId(i) == id)
                {
                    return i;
                }
            }
            return -1;
        }

        private void toggle(View contentParent)
        {
            if (mListViewWrapper == null)
            {
                throw new Java.Lang.IllegalStateException("No ListView set!");
            }


            bool isVisible = contentParent.Visibility == ViewStates.Visible;
            bool shouldCollapseOther = !isVisible && mLimit > 0 && mExpandedIds.Count >= mLimit;
            if (shouldCollapseOther)
            {
                long firstId = mExpandedIds[0];

                int firstPosition = findPositionForId(firstId);
                View firstEV = getContentParent(firstPosition);
                if (firstEV != null)
                {
                    ExpandCollapseHelper.animateCollapsing(firstEV);
                }
                mExpandedIds.Remove(firstId);

                if (mExpandCollapseListener != null)
                {
                    mExpandCollapseListener.onItemCollapsed(firstPosition);
                }
            }

            long id = (long)contentParent.Tag;
            int position = findPositionForId(id);
            if (isVisible)
            {
                ExpandCollapseHelper.animateCollapsing(contentParent);
                mExpandedIds.Remove(id);

                if (mExpandCollapseListener != null)
                {
                    mExpandCollapseListener.onItemCollapsed(position);
                }

            }
            else
            {
                ExpandCollapseHelper.animateExpanding(contentParent, mListViewWrapper);
                mExpandedIds.Add(id);

                if (mExpandCollapseListener != null)
                {
                    mExpandCollapseListener.onItemExpanded(position);
                }
            }
        }

        public interface ExpandCollapseListener
        {

            void onItemExpanded(int position);

            void onItemCollapsed(int position);
        }

        private class RootView : LinearLayout
        {

            private ViewGroup mTitleViewGroup;
            private ViewGroup mContentViewGroup;

            public RootView(Context context)
                : base(context)
            {
                //super(context);
                init();
            }

            private void init()
            {
                Orientation = Orientation.Vertical;

                mTitleViewGroup = new FrameLayout(Context);
                mTitleViewGroup.Id = DEFAULTTITLEPARENTRESID;
                AddView(mTitleViewGroup);

                mContentViewGroup = new FrameLayout(Context);
                mContentViewGroup.Id = (DEFAULTCONTENTPARENTRESID);
                AddView(mContentViewGroup);
            }
        }

        private class ViewHolder : Java.Lang.Object
        {
            public ViewGroup titleParent;
            public ViewGroup contentParent;
            public View titleView;
            public View contentView;
        }

        private class ExpandCollapseAnimatorHelper:AnimatorListenerAdapter
        {
            private View mView;
            public ExpandCollapseAnimatorHelper(View view)
            {
                mView=view;
            }
            public override void OnAnimationEnd(Animator animation)
            {
 	             mView.Visibility=ViewStates.Gone;
            }
        }

        class animateExpandingInnerClass : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
        {
            int listViewHeight  ;
            int listViewBottomPadding  ;
            View v ;
            private ListViewWrapper mlist;
            private View mview;


            public animateExpandingInnerClass(View view,ListViewWrapper listViewWrapper)
            {
                mlist = listViewWrapper;
                mview = view;
                  listViewHeight = listViewWrapper.getListView().Height;
                  listViewBottomPadding = listViewWrapper.getListView().PaddingBottom;
                  v = ExpandCollapseHelper.findDirectChild(view, listViewWrapper.getListView());
            }

            public void OnAnimationUpdate(ValueAnimator animation)
            {
                int bottom = v.Bottom;
                if (bottom > listViewHeight)
                {
                    int top = v.Top;
                    if (top > 0)
                    {
                        mlist.smoothScrollBy(Math.Min(bottom - listViewHeight + listViewBottomPadding, top), 0);
                    }
                }
            }

        }

        private class ExpandCollapseHelper
        {

            public static void animateCollapsing(View view)
            {
                int origHeight = view.Height;

                ValueAnimator animator = createHeightAnimator(view, origHeight, 0);
                //throw new NotImplementedException();
                //animator.addListener(
                //        new AnimatorListenerAdapter() {

                //            @Override
                //            public void onAnimationEnd( Animator animation) {
                //                view.setVisibility(View.GONE);
                //            }
                //        }
                //);
                animator.AddListener(new ExpandCollapseAnimatorHelper(view));
                 
                animator.Start();
            }

            public static void animateExpanding(View view, ListViewWrapper listViewWrapper)
            {
                view.Visibility = ViewStates.Visible;

                View parent = (View)view.Parent;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(parent.MeasuredWidth - parent.PaddingLeft - parent.PaddingRight, MeasureSpecMode.AtMost);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                view.Measure(widthSpec, heightSpec);

                ValueAnimator animator = createHeightAnimator(view, 0, view.MeasuredHeight);
                //throw new NotImplementedException();
                //animator.addUpdateListener(
                //        new ValueAnimator.AnimatorUpdateListener() {
                //            int listViewHeight = listViewWrapper.getListView().getHeight();
                //            int listViewBottomPadding = listViewWrapper.getListView().getPaddingBottom();
                //            View v = findDirectChild(view, listViewWrapper.getListView());

                //            @Override
                //            public void onAnimationUpdate( ValueAnimator animation) {
                //                int bottom = v.getBottom();
                //                if (bottom > listViewHeight) {
                //                    int top = v.getTop();
                //                    if (top > 0) {
                //                        listViewWrapper.smoothScrollBy(Math.min(bottom - listViewHeight + listViewBottomPadding, top), 0);
                //                    }
                //                }
                //            }
                //        }
                //);

                animator.AddUpdateListener(new animateExpandingInnerClass(view, listViewWrapper));
                animator.Start();
            }

            public static ValueAnimator createHeightAnimator(View view, int start, int end)
            {
                ValueAnimator animator = ValueAnimator.OfInt(start, end);
                //throw new NotImplementedException();
                //animator.addUpdateListener(
                //        new ValueAnimator.AnimatorUpdateListener() {

                //            @Override
                //            public void onAnimationUpdate( ValueAnimator animation) {
                //                int value = (Integer) animation.getAnimatedValue();

                //                LayoutParams layoutParams = view.getLayoutParams();
                //                layoutParams.height = value;
                //                view.setLayoutParams(layoutParams);
                //            }
                //        }
                //);
                animator.AddUpdateListener(new createHeightAnimatorInnerClass(view));
                return animator;
            }

            //@NonNull
            public static View findDirectChild(View view, ViewGroup listView)
            {
                View result = view;
                View parent = (View)result.Parent;
                while (!parent.Equals(listView))
                {
                    result = parent;
                    parent = (View)result.Parent;
                }
                return result;
            }
        }

        class createHeightAnimatorInnerClass : Java.Lang.Object,ValueAnimator.IAnimatorUpdateListener
        {
            View mView;

            public createHeightAnimatorInnerClass(View view)
            {
                mView = view;
            }
            public void OnAnimationUpdate(ValueAnimator animation)
            {
                int value = (int)animation.AnimatedValue;

                ViewGroup.LayoutParams layoutParams = mView.LayoutParameters;
                layoutParams.Height = value;
                mView.LayoutParameters=layoutParams;
            }
        }

        private class TitleViewOnClickListener : Java.Lang.Object, View.IOnClickListener
        {

            private View mContentParent;
            private ExpandableListItemAdapter<T> minst;

            public TitleViewOnClickListener(View contentParent,ExpandableListItemAdapter<T> instance )
            {
                mContentParent = contentParent;
                minst = instance;
            }

            //@Override
            public void OnClick(View view)
            {
                minst.toggle(mContentParent);
            }
        }
    }
}