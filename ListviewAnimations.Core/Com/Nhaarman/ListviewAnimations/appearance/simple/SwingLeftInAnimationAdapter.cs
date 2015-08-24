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

//import com.nhaarman.listviewanimations.appearance.SingleAnimationAdapter;
//import com.nineoldandroids.animation.Animator;
//import com.nineoldandroids.animation.ObjectAnimator;

/**
 * An implementation of the AnimationAdapter class which applies a
 * swing-in-from-the-left-animation to views.
 */

using Android.Animation;
using Android.Views;
using Android.Widget;
namespace Com.Nhaarman.ListviewAnimations.Appearance.Simple
{
    //@SuppressWarnings("UnusedDeclaration")
    public class SwingLeftInAnimationAdapter : SingleAnimationAdapter
    {

        private static readonly string TRANSLATION_X = "translationX";

        public SwingLeftInAnimationAdapter(BaseAdapter baseAdapter)
            : base(baseAdapter)
        {
            //super(baseAdapter);
        }


        protected override Animator getAnimator(ViewGroup parent, View view)
        {
            return ObjectAnimator.OfFloat(view, TRANSLATION_X, 0 - parent.Width, 0);
        }
    }
}