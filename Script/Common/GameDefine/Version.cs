/********************************************************************************
 *	文件名：	Version.cs
 *	全路径：	\Script\GlobeDefine\Version.cs
 *	创建人：	wanghua
 *	创建时间：2013-12-20
 *
 *	功能说明：Version打版本用
 *	修改记录：
*********************************************************************************/

/*
     * 客户端版本号格式：A.B.C.D
     * ABCD的说明如下：
     * A-游戏大版本号（GameVersion），B-代码版本号（ProgramVersion），C-公共资源版本号（PublicResourceVersion）,D-内部资源版本号（PrivateResourceVersion）.
     * 其中AB写在代码中，即下面两个，C配置在公共PublicConfig.txt文件中，D记录在UserConfigData.cs的UserConfigData类中
*/
public enum VERSION
{

    GameVersion = 0,
    ProgramVersion = 5,
    
}