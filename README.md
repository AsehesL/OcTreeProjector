# OcTreeProjector

该插件通过预先将需要应用Projector效果的mesh信息存入八叉树，并在渲染Projector时只渲染投射到的部分的mesh，解决了部分情况下Projector投影的mesh过大导致顶点数浪费的情况。

![octree](Doc/octree.JPG)

![ocproj1](Doc/ocproj1.JPG)

只渲染投影到的mesh：

![ocproj2](Doc/ocproj2.JPG)



使用说明：

一、右键创建OcTree：

![ocproj3](Doc/ocproj3.jpg)

二、选择场景中需要应用投影效果的物体，如果其子物体也需要，则勾选包含子物体，点击创建

![ocproj4](Doc/ocproj4.JPG)

三、在Resources目录下生成八叉树对象

![ocproj5](Doc/ocproj5.JPG)

四、添加OT Projector脚本，设置Projector参数，并在OcTree选项处选择刚刚生成的OcTree对象

![ocproj6](Doc/ocproj6.JPG)