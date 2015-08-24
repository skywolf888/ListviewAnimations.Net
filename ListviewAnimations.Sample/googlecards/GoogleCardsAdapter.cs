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

//package com.haarman.listviewanimations.googlecards;

//import android.content.Context;
//import android.graphics.Bitmap;
//import android.graphics.BitmapFactory;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.ImageView;
//import android.widget.TextView;

//import com.haarman.listviewanimations.R;
//import com.haarman.listviewanimations.util.BitmapCache;
//import com.nhaarman.listviewanimations.ArrayAdapter;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations;
using ListviewAnimations.Sample.gridview;
using ListviewAnimations.Sample.Util;
namespace ListviewAnimations.Sample.googlecards
{
    public class GoogleCardsAdapter : Com.Nhaarman.ListviewAnimations.ArrayAdapter<int>
    {

        private Context mContext;
        private BitmapCache mMemoryCache;

        GoogleCardsAdapter(Context context)
        {
            mContext = context;
            mMemoryCache = new BitmapCache();
        }
        private void setImageView(ViewHolder viewHolder, int position)
        {
            int imageResId;
            switch ((int)GetItem(position) % 5)
            {
                case 0:
                    imageResId = Resource.Drawable.img_nature1;
                    break;
                case 1:
                    imageResId = Resource.Drawable.img_nature2;
                    break;
                case 2:
                    imageResId = Resource.Drawable.img_nature3;
                    break;
                case 3:
                    imageResId = Resource.Drawable.img_nature4;
                    break;
                default:
                    imageResId = Resource.Drawable.img_nature5;
                    break;
            }
            
            Bitmap bitmap = getBitmapFromMemCache(imageResId);
            if (bitmap == null)
            {
                bitmap = BitmapFactory.DecodeResource(mContext.Resources, imageResId);
                addBitmapToMemoryCache(imageResId, bitmap);
            }
            viewHolder.imageView.SetImageBitmap(bitmap);
        }

        private void addBitmapToMemoryCache(int key, Bitmap bitmap)
        {
            if (getBitmapFromMemCache(key) == null)
            {
                mMemoryCache.Put(key, bitmap);
            }
        }

        private Bitmap getBitmapFromMemCache(int key)
        {            
            return (Bitmap)mMemoryCache.Get(key);
        }

        //@SuppressWarnings({"PackageVisibleField", "InstanceVariableNamingConvention"})
        private class ViewHolder:Java.Lang.Object
        {
            public TextView textView;
            public ImageView imageView;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(mContext).Inflate(Resource.Layout.activity_googlecards_card, parent, false);

                viewHolder = new ViewHolder();
                viewHolder.textView = (TextView)view.FindViewById(Resource.Id.activity_googlecards_card_textview);
                view.Tag=viewHolder;
				
                viewHolder.imageView = (ImageView)view.FindViewById(Resource.Id.activity_googlecards_card_imageview);
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
            }

            viewHolder.textView.Text=mContext.GetString(Resource.String.card_number, (int)GetItem(position) + 1);
            setImageView(viewHolder, position);

            return view;
        }
    }
}