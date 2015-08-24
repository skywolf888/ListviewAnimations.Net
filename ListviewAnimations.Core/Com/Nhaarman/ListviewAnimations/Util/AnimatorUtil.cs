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

//package com.nhaarman.listviewanimations.util;

//import android.support.annotation.NonNull;

//import com.nineoldandroids.animation.Animator;
using Android.Animation;
namespace Com.Nhaarman.ListviewAnimations.Util
{
    public class AnimatorUtil
    {

        private AnimatorUtil()
        {
        }

        /**
         * Merges given Animators into one array.
         */
        //@NonNull
        public static Animator[] concatAnimators(Animator[] childAnimators, Animator[] animators, Animator alphaAnimator)
        {
            Animator[] allAnimators = new Animator[childAnimators.Length + animators.Length + 1];
            int i;

            for (i = 0; i < childAnimators.Length; ++i)
            {
                allAnimators[i] = childAnimators[i];
            }

            foreach (Animator animator in animators)
            {
                allAnimators[i] = animator;
                ++i;
            }

            allAnimators[allAnimators.Length - 1] = alphaAnimator;
            return allAnimators;
        }

    }
}