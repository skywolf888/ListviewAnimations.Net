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

//import android.support.annotation.IdRes;
//import android.support.annotation.NonNull;
//import android.view.View;
using Android.Views;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.dragdrop
{
    public class TouchViewDraggableManager : DraggableManager
    {

        //@IdRes
        private int mTouchViewResId;

        public TouchViewDraggableManager(int touchViewResId)
        {
            mTouchViewResId = touchViewResId;
        }

        //@Override
        public bool isDraggable(View view, int position, float x, float y)
        {
            View touchView = view.FindViewById(mTouchViewResId);
            if (touchView != null)
            {
                bool xHit = touchView.Left <= x && touchView.Right >= x;
                bool yHit = touchView.Top <= y && touchView.Bottom >= y;
                return xHit && yHit;
            }
            else
            {
                return false;
            }
        }
    }
}