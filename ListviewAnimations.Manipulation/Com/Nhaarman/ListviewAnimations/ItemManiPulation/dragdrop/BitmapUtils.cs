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

//import android.graphics.Bitmap;
//import android.graphics.Canvas;
//import android.support.annotation.NonNull;
//import android.view.View;
using Android.Graphics;
using Android.Views;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{
    class BitmapUtils
    {

        private BitmapUtils()
        {
        }

        /**
         * Returns a bitmap showing a screenshot of the view passed in.
         */
        //@NonNull
        public static Bitmap getBitmapFromView(View v)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(v.MeasuredWidth, v.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            v.Draw(canvas);
            return bitmap;
        }

    }
}