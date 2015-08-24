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

//import android.os.Bundle;
//import android.os.Parcelable;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

//import com.nhaarman.listviewanimations.BaseAdapterDecorator;
//import com.nhaarman.listviewanimations.util.AnimatorUtil;
//import com.nhaarman.listviewanimations.util.ListViewWrapper;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.ObjectAnimator;

/**
 * A {@link BaseAdapterDecorator} class which applies multiple {@link Animator}s at once to views when they are first shown. The Animators applied include the animations specified
 * in {@link #getAnimators(ViewGroup, View)}, plus an alpha transition.
 */

using Android.Animation;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.Util;

namespace Com.Nhaarman.ListviewAnimations.Appearance
{
    public abstract class AnimationAdapter : BaseAdapterDecorator
    {

        /**
         * Saved instance state key for the ViewAniamt
         */
        private static readonly string SAVEDINSTANCESTATE_VIEWANIMATOR = "savedinstancestate_viewanimator";

        /**
         * Alpha property
         */
        private static readonly string ALPHA = "alpha";

        /**
         * The ViewAnimator responsible for animating the Views.
         */
        //@Nullable
        private ViewAnimator mViewAnimator;

        /**
         * Whether this instance is the root AnimationAdapter. When this is set to false, animation is not applied to the views, since the wrapper AnimationAdapter will take care of
         * that.
         */
        private bool mIsRootAdapter;

        /**
         * If the AbsListView is an instance of GridView, this boolean indicates whether the GridView is possibly measuring the view.
         */
        private bool mGridViewPossiblyMeasuring;

        /**
         * The position of the item that the GridView is possibly measuring.
         */
        private int mGridViewMeasuringPosition;

        /**
         * Creates a new AnimationAdapter, wrapping given BaseAdapter.
         *
         * @param baseAdapter the BaseAdapter to wrap.
         */
        protected AnimationAdapter(BaseAdapter baseAdapter)
            : base(baseAdapter)
        {
            //super(baseAdapter);

            mGridViewPossiblyMeasuring = true;
            mGridViewMeasuringPosition = -1;
            mIsRootAdapter = true;

            if (baseAdapter is AnimationAdapter)
            {
                ((AnimationAdapter)baseAdapter).setIsWrapped();
            }
        }

        //@Override
        public override void setListViewWrapper(IListViewWrapper listViewWrapper)
        {
            base.setListViewWrapper(listViewWrapper);
            mViewAnimator = new ViewAnimator(listViewWrapper);
        }

        /**
         * Sets whether this instance is wrapped by another instance of AnimationAdapter. If called, this instance will not apply any animations to the views, since the wrapper
         * AnimationAdapter handles that.
         */
        private void setIsWrapped()
        {
            mIsRootAdapter = false;
        }

        /**
         * Call this method to reset animation status on all views. The next time {@link #notifyDataSetChanged()} is called on the base adapter, all views will animate again.
         */
        public void reset()
        {
            if (getListViewWrapper() == null)
            {
                throw new Java.Lang.IllegalStateException("Call setAbsListView() on this AnimationAdapter first!");
            }

            //assert mViewAnimator != null;


            mViewAnimator.reset();

            mGridViewPossiblyMeasuring = true;
            mGridViewMeasuringPosition = -1;

            if (getDecoratedBaseAdapter() is AnimationAdapter)
            {
                ((AnimationAdapter)getDecoratedBaseAdapter()).reset();
            }
        }

        /**
         * Returns the {@link com.nhaarman.listviewanimations.appearance.ViewAnimator} responsible for animating the Views in this adapter.
         */
        //@Nullable
        public ViewAnimator getViewAnimator()
        {
            return mViewAnimator;
        }

        //@NonNull
        //@Override
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (mIsRootAdapter)
            {
                if (getListViewWrapper() == null)
                {
                    throw new Java.Lang.IllegalStateException("Call setAbsListView() on this AnimationAdapter first!");
                }

                //assert mViewAnimator != null;

                if (convertView != null)
                {
                    mViewAnimator.cancelExistingAnimation(convertView);

                }
            }

            View itemView = base.GetView(position, convertView, parent);

            if (mIsRootAdapter)
            {
                animateViewIfNecessary(position, itemView, parent);
            }
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

            //assert mViewAnimator != null;

            /* GridView measures the first View which is returned by getView(int, View, ViewGroup), but does not use that View.
               On KitKat, it does this actually multiple times.
               Therefore, we animate all these first Views, and reset the last animated position when we suspect GridView is measuring. */
            mGridViewPossiblyMeasuring = mGridViewPossiblyMeasuring && (mGridViewMeasuringPosition == -1 || mGridViewMeasuringPosition == position);

            if (mGridViewPossiblyMeasuring)
            {
                mGridViewMeasuringPosition = position;
                mViewAnimator.setLastAnimatedPosition(-1);

            }

            Animator[] childAnimators;
            if (getDecoratedBaseAdapter() is AnimationAdapter)
            {
                childAnimators = ((AnimationAdapter)getDecoratedBaseAdapter()).getAnimators(parent, view);
            }
            else
            {
                childAnimators = new Animator[0];
            }
            Animator[] animators = getAnimators(parent, view);
            Animator alphaAnimator = ObjectAnimator.OfFloat(view, ALPHA, 0, 1);

            Animator[] concatAnimators = AnimatorUtil.concatAnimators(childAnimators, animators, alphaAnimator);
            mViewAnimator.animateViewIfNecessary(position, view, concatAnimators);
        }

        /**
         * Returns the Animators to apply to the views. In addition to the returned Animators, an alpha transition will be applied to the view.
         *
         * @param parent The parent of the view
         * @param view   The view that will be animated, as retrieved by getView().
         */
        //@NonNull
        public abstract Animator[] getAnimators(ViewGroup parent, View view);

        /**
         * Returns a Parcelable object containing the AnimationAdapter's current dynamic state.
         */

        public IParcelable onSaveInstanceState()
        {
            Bundle bundle = new Bundle();

            if (mViewAnimator != null)
            {
                bundle.PutParcelable(SAVEDINSTANCESTATE_VIEWANIMATOR, mViewAnimator.onSaveInstanceState());
            }

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
                if (mViewAnimator != null)
                {
                    mViewAnimator.onRestoreInstanceState((IParcelable)bundle.GetParcelable(SAVEDINSTANCESTATE_VIEWANIMATOR));
                }
            }
        }
    }
}