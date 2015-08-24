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

//package com.nhaarman.listviewanimations.itemmanipulation.dragdrop;

//import android.graphics.drawable.BitmapDrawable;
//import android.support.annotation.NonNull;
//import android.view.MotionEvent;
//import android.view.View;

using Android.Graphics.Drawables;
using Android.Views;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{

    /**
     * A Drawable which represents a dragging {@link View}.
     */
    internal class HoverDrawable : BitmapDrawable
    {

        /**
         * The original y coordinate of the top of given {@code View}.
         */
        private float mOriginalY;

        /**
         * The original y coordinate of the position that was touched.
         */
        private float mDownY;

        /**
         * The distance the {@code ListView} has been scrolling while this {@code HoverDrawable} is alive.
         */
        private float mScrollDistance;

        /**
         * Creates a new {@code HoverDrawable} for given {@link View}, using given {@link MotionEvent}.
         *
         * @param view the {@code View} to represent.
         * @param ev   the {@code MotionEvent} to use as down position.
         */
        internal HoverDrawable(View view, MotionEvent ev)
            : this(view, ev.GetY())
        {
            //this(view, ev.GetY());
        }

        /**
         * Creates a new {@code HoverDrawable} for given {@link View}, using given {@link MotionEvent}.
         *
         * @param view  the {@code View} to represent.
         * @param downY the y coordinate of the down event.
         */
        internal HoverDrawable(View view, float downY)
            : base(view.Resources, BitmapUtils.getBitmapFromView(view))
        {

            mOriginalY = view.Top;
            mDownY = downY;

            SetBounds(view.Left, view.Top, view.Right, view.Bottom);
        }

        /**
         * Calculates the new position for this {@code HoverDrawable} using given {@link MotionEvent}.
         *
         * @param ev the {@code MotionEvent}.
         *           {@code ev.getActionMasked()} should typically equal {@link MotionEvent#ACTION_MOVE}.
         */
        public void handleMoveEvent(MotionEvent ev)
        {
            int top = (int)(mOriginalY - mDownY + ev.GetY() + mScrollDistance);
            setTop(top);
        }

        /**
         * Updates the original y position of the view, and calculates the scroll distance.
         *
         * @param mobileViewTopY the top y coordinate of the mobile view this {@code HoverDrawable} represents.
         */
        internal void onScroll(float mobileViewTopY)
        {
            mScrollDistance += mOriginalY - mobileViewTopY;
            mOriginalY = mobileViewTopY;
        }

        /**
         * Returns whether the user is currently dragging this {@code HoverDrawable} upwards.
         *
         * @return true if dragging upwards.
         */
        internal bool isMovingUpwards()
        {
            return mOriginalY > Bounds.Top;
        }

        /**
         * Returns the number of pixels between the original y coordinate of the view, and the current y coordinate.
         * A negative value means this {@code HoverDrawable} is moving upwards.
         *
         * @return the number of pixels.
         */
        internal int getDeltaY()
        {
            return (int)(Bounds.Top - mOriginalY);
        }

        /**
         * Returns the top coordinate of this {@code HoverDrawable}.
         */
        internal int getTop()
        {
            return Bounds.Top;
        }

        /**
         * Sets the top coordinate of this {@code HoverDrawable}.
         */
        internal void setTop(int top)
        {
            SetBounds(Bounds.Left, top, Bounds.Left + IntrinsicWidth, top + IntrinsicHeight);
        }

        /**
         * Shifts the original y coordinates of this {@code HoverDrawable} {code height} pixels upwards or downwards,
         * depending on the move direction.
         *
         * @param height the number of pixels this {@code HoverDrawable} should be moved. Should be positive.
         */
        internal void shift(int height)
        {
            int shiftSize = isMovingUpwards() ? -height : height;
            mOriginalY += shiftSize;
            mDownY += shiftSize;
        }
    }
}