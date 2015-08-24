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
//package com.nhaarman.listviewanimations.appearance.simple;

//import android.support.annotation.NonNull;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;

using Android.Animation;
using Android.Views;
//import com.nhaarman.listviewanimations.appearance.AnimationAdapter;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.ObjectAnimator;
using Android.Widget;
namespace Com.Nhaarman.ListviewAnimations.Appearance.Simple
{
    public class ScaleInAnimationAdapter : AnimationAdapter
    {

        private static readonly float DEFAULT_SCALE_FROM = 0.8f;

        private static readonly string SCALE_X = "scaleX";
        private static readonly string SCALE_Y = "scaleY";

        private readonly float mScaleFrom;

        public ScaleInAnimationAdapter(BaseAdapter baseAdapter)
            : this(baseAdapter, DEFAULT_SCALE_FROM)
        {
            //this(baseAdapter, DEFAULT_SCALE_FROM);
        }

        public ScaleInAnimationAdapter(BaseAdapter baseAdapter, float scaleFrom)
            : base(baseAdapter)
        {
            //super(baseAdapter);
            mScaleFrom = scaleFrom;
        }

        public override Animator[] getAnimators(ViewGroup parent, View view)
        {
            ObjectAnimator scaleX = ObjectAnimator.OfFloat(view, SCALE_X, mScaleFrom, 1f);
            ObjectAnimator scaleY = ObjectAnimator.OfFloat(view, SCALE_Y, mScaleFrom, 1f);
            return new ObjectAnimator[] { scaleX, scaleY };
        }
    }
}