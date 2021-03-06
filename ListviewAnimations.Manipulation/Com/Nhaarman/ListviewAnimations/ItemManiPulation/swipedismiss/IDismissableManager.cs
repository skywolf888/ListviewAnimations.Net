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
//
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.swipedismiss
{

    /**
     * An interface to specify whether certain items can or cannot be dismissed.
     */
    public interface IDismissableManager
    {

        /**
         * Returns whether the item for given id and position can be dismissed.
         * @param id the id of the item.
         * @param position the position of the item.
         * @return true if the item can be dismissed, false otherwise.
         */
        bool isDismissable(long id, int position);
    }
}