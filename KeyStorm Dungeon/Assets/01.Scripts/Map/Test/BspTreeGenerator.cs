using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class BspTreeGenerator
{
    int minSplitSize;

    public BspTreeGenerator(int minSplitSize)
    {
        this.minSplitSize = minSplitSize;
    }

    public BspNode GenerateTree(RectInt rect, int depth)
    {
        BspNode root = new BspNode(rect);
        SplitNode(root, depth - 1);
        return root;
    }

    void SplitNode(BspNode node, int depth)
    {
        if (depth <= 0) return;

        bool isSplitHeight = Random.value > SplitNodeHeight;

        if (node.rect.width > node.rect.height)
            isSplitHeight = false;
        else if (node.rect.height > node.rect.width)
            isSplitHeight = true;

        int maxSize = isSplitHeight ? node.rect.height : node.rect.width;

        if (maxSize <= minSplitSize * 2)
            return;

        int splitPoint = Random.Range(minSplitSize, maxSize - minSplitSize);

        if(isSplitHeight)
        {
            RectInt leffRect = new RectInt(node.rect.x, node.rect.y, node.rect.width, splitPoint);
            RectInt rightRect = new RectInt(node.rect.x, node.rect.y + splitPoint, node.rect.width, node.rect.height - splitPoint);
            node.left = new BspNode(leffRect);
            node.right = new BspNode(rightRect);
        }
        else
        {
            RectInt leffRect = new RectInt(node.rect.x, node.rect.y, splitPoint, node.rect.height);
            RectInt rightRect = new RectInt(node.rect.x + splitPoint, node.rect.y, node.rect.width - splitPoint, node.rect.height);
            node.left = new BspNode(leffRect);
            node.right = new BspNode(rightRect);
        }

        SplitNode(node.left, depth);
        SplitNode(node.right, depth);
    }
}
