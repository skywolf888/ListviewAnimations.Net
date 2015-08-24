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

//import android.content.Context;
//import android.content.res.Resources;
//import android.content.res.TypedArray;
//import android.graphics.Canvas;
//import android.graphics.Paint;
//import android.support.annotation.ColorRes;
//import android.support.annotation.NonNull;
//import android.support.annotation.Nullable;
//import android.util.AttributeSet;
//import android.util.TypedValue;
//import android.view.View;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;


namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{

    /**
     * A {@code View} which shows a number of colored dots in a grid.
     */
    //@SuppressWarnings("UnnecessaryFullyQualifiedName")
    public class GripView : View
    {

        public static readonly int DEFAULT_DOT_COLOR = Android.Resource.Color.DarkerGray;
        public static readonly float DEFAULT_DOT_SIZE_RADIUS_DP = 2;
        public static readonly int DEFAULT_COLUMN_COUNT = 2;

        private static readonly int[] ATTRS = { Android.Resource.Attribute.Color };

        /**
         * The {@code Paint} that is used to draw the dots.
         */
        private Paint mDotPaint;

        /**
         * The radius in pixels of the dots.
         */
        private float mDotSizeRadiusPx;

        /**
         * The calculated top padding to make sure the dots are centered. Calculated in {@link #onSizeChanged(int, int, int, int)}.
         */
        private float mPaddingTop;

        /**
         * The number of columns.
         */
        private int mColumnCount = DEFAULT_COLUMN_COUNT;

        /**
         * The number of rows. Calculated in {@link #onSizeChanged(int, int, int, int)}.
         */
        private int mRowCount;

        public GripView(Context context)
            : this(context, null)
        {

        }

        public GripView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {

        }

        public GripView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            mDotPaint = new Paint(PaintFlags.AntiAlias);
            Color color = Resources.GetColor(DEFAULT_DOT_COLOR);
            if (attrs != null)
            {
                TypedArray a = context.ObtainStyledAttributes(attrs, ATTRS);
                color = a.GetColor(0, color);
                a.Recycle();
            }
            mDotPaint.Color = color;

            Resources r = context.Resources;
            mDotSizeRadiusPx = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, DEFAULT_DOT_SIZE_RADIUS_DP, r.DisplayMetrics);
        }

        /**
         * Sets the color of the dots. Defaults to {@link #DEFAULT_DOT_COLOR}.
         */
        public void setColor(int colorResId)
        {
            mDotPaint.Color = (Resources.GetColor(colorResId));
        }

        /**
         * Sets the radius in pixels of the dots. Defaults to {@value #DEFAULT_DOT_SIZE_RADIUS_DP} dp.
         */
        public void setDotSizeRadiusPx(float dotSizeRadiusPx)
        {
            mDotSizeRadiusPx = dotSizeRadiusPx;
        }

        /**
         * Sets the number of horizontal dots. Defaults to {@value #DEFAULT_COLUMN_COUNT}.
         */
        public void setColumnCount(int columnCount)
        {
            mColumnCount = columnCount;
            RequestLayout();
        }

        //@SuppressWarnings("ParameterNameDiffersFromOverriddenParameter")
        //@Override
        protected override void OnSizeChanged(int width, int height, int oldWidth, int oldHeight)
        {
            base.OnSizeChanged(width, height, oldWidth, oldHeight);

            mRowCount = (int)((height - PaddingTop - PaddingBottom) / (mDotSizeRadiusPx * 4));
            mPaddingTop = (height - mRowCount * mDotSizeRadiusPx * 2 - (mRowCount - 1) * mDotSizeRadiusPx * 2) / 2;
        }

        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = MeasureSpec.MakeMeasureSpec(PaddingLeft + PaddingRight + (int)(mColumnCount * (mDotSizeRadiusPx * 4 - 2)), MeasureSpecMode.Exactly);
            base.OnMeasure(width, heightMeasureSpec);
        }

        //@Override
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            for (int i = 0; i < mColumnCount; i++)
            {
                float x = PaddingLeft + i * 2 * mDotSizeRadiusPx * 2;
                for (int j = 0; j < mRowCount; j++)
                {
                    float y = mPaddingTop + j * 2 * mDotSizeRadiusPx * 2;
                    canvas.DrawCircle(x + mDotSizeRadiusPx, y + mDotSizeRadiusPx, mDotSizeRadiusPx, mDotPaint);
                }
            }
        }
    }
}
