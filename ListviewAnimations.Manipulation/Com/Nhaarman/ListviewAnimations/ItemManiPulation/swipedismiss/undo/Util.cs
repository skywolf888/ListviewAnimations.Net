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
//package com.nhaarman.listviewanimations.itemmanipulation.swipedismiss.undo;

//import android.support.annotation.NonNull;

//import java.util.ArrayList;
//import java.util.Collection;
//import java.util.Collections;
//import java.util.Iterator;
//import java.util.List;

using System.Collections.Generic;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss.undo
{

    class Util
    {

        private Util()
        {
        }

        //@NonNull
        internal static ICollection<int> processDeletions(ICollection<int> positions, int[] dismissedPositions)
        {
            List<int> dismissedList = new List<int>();
            foreach (int position in dismissedPositions)
            {
                dismissedList.Add(position);
            }
            return processDeletions(positions, dismissedList);
        }

        /**
         * Removes positions in {@code dismissedPositions} from {@code positions}, and shifts the remaining positions accordingly.
         *
         * @param positions          the list of positions to remove from.
         * @param dismissedPositions the list of positions to remove.
         *
         * @return a new {@link java.util.Collection} instance, containing the resulting positions.
         */
        //@NonNull
        internal static ICollection<int> processDeletions(ICollection<int> positions, List<int> dismissedPositions)
        {
            List<int> result = new List<int>(positions);
            dismissedPositions.Sort();
            dismissedPositions.Reverse();
            //Collections.sort(dismissedPositions, Collections.reverseOrder());
            ICollection<int> newUndoPositions = new List<int>();
            foreach (int position in dismissedPositions)
            {
                //for (IEnumerator<int> iterator = result.GetEnumerator(); iterator.MoveNext(); )
                //{
                    
                //    int undoPosition = iterator.Current;
                //    if (undoPosition > position)
                //    {
                //        iterator.remove();
                        
                //        newUndoPositions.Add(undoPosition - 1);
                //    }
                //    else if (undoPosition == position)
                //    {
                //        iterator.remove();
                //    }
                //    else
                //    {
                //        newUndoPositions.Add(undoPosition);
                //    }
                //}
                foreach (int item in dismissedPositions)
                {
                    if (item > position)
                    {
                        dismissedPositions.Remove(item);
                        newUndoPositions.Add(item - 1);
                    }
                    else if (item == position)
                    {
                        dismissedPositions.Remove(item);
                    }
                    else {
                        newUndoPositions.Add(item);
                    }

                }
                result.Clear();
                result.AddRange(newUndoPositions);
                newUndoPositions.Clear();
            }

            return result;
        }
    }
}