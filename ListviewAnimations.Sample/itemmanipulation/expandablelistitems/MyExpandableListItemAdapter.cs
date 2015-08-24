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

//package com.haarman.listviewanimations.itemmanipulation.expandablelistitems;

//import android.content.Context;
//import android.graphics.Bitmap;
//import android.graphics.BitmapFactory;
//import android.support.annotation.NonNull;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.ImageView;
//import android.widget.TextView;

//import com.haarman.listviewanimations.R;
//import com.haarman.listviewanimations.util.BitmapCache;
//import com.nhaarman.listviewanimations.itemmanipulation.expandablelistitem.ExpandableListItemAdapter;

using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Com.Nhaarman.ListviewAnimations.ItemManiPulation.expandablelistitem;
using ListviewAnimations.Sample.Util;
namespace ListviewAnimations.Sample.itemmanipulation.expandablelistitems
{
public class MyExpandableListItemAdapter : ExpandableListItemAdapter<int> {

    private Context mContext;
    private BitmapCache mMemoryCache;

    /**
     * Creates a new ExpandableListItemAdapter with the specified list, or an empty list if
     * items == null.
     */
    public MyExpandableListItemAdapter(Context context) 
		:base(context, Resource.Layout.activity_expandablelistitem_card, Resource.Id.activity_expandablelistitem_card_title, Resource.Id.activity_expandablelistitem_card_content)
    {
        
        mContext = context;
        mMemoryCache = new BitmapCache();

        for (int i = 0; i < 100; i++) {
            add(i);
        }
    }

    //@NonNull
    //@Override
    public override View getTitleView(  int position,   View convertView,     ViewGroup parent) {
        TextView tv = (TextView) convertView;
        if (tv == null) {
            tv = new TextView(mContext);
        }
        tv.Text=mContext.GetString(Resource.String.expandorcollapsecard, (int) GetItem(position));
        return tv;
    }

    //@NonNull
    //@Override
    public override View getContentView(  int position,   View convertView,     ViewGroup parent) {
        ImageView imageView = (ImageView) convertView;
        if (imageView == null) {
            imageView = new ImageView(mContext);
            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
			
        }

        int imageResId;
        switch ((int)GetItem2(position) % 5) {
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
        if (bitmap == null) {
            bitmap = BitmapFactory.DecodeResource(mContext.Resources, imageResId);
            addBitmapToMemoryCache(imageResId, bitmap);
        }
        imageView.SetImageBitmap(bitmap);

        return imageView;
    }

    private void addBitmapToMemoryCache(  int key,   Bitmap bitmap) {
        if (getBitmapFromMemCache(key) == null) {
            mMemoryCache.Put (key, bitmap);
        }
    }

    private Bitmap getBitmapFromMemCache(  int key) {
        return (Bitmap) mMemoryCache.Get(key);
    }
}
}