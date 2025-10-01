// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:14,ufog:False,aust:False,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:35553,y:35884,varname:node_3138,prsc:2|emission-3744-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:3591,x:31830,y:36092,ptovrint:True,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7014,x:29753,y:33174,varname:_OffsetTex,prsc:0,ntxv:0,isnm:False|TEX-8991-TEX;n:type:ShaderForge.SFN_Vector1,id:8085,x:29753,y:33362,varname:node_8085,prsc:0,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:2855,x:32299,y:32570,varname:_node_2855,prsc:0,ntxv:0,isnm:False|UVIN-3880-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Slider,id:2077,x:29753,y:33550,ptovrint:False,ptlb:Offset1,ptin:_Offset1,varname:_Offset1,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.5,cur:0,max:0.5;n:type:ShaderForge.SFN_Tex2d,id:5905,x:32312,y:32896,varname:_node_2713,prsc:0,ntxv:0,isnm:False|UVIN-2831-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:9527,x:32336,y:33374,varname:_node_9641,prsc:0,ntxv:0,isnm:False|UVIN-6576-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:2299,x:32349,y:33700,varname:_node_7397,prsc:0,ntxv:0,isnm:False|UVIN-5309-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_TexCoord,id:2396,x:31036,y:34521,varname:node_2396,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:7584,x:29795,y:35175,varname:_OffsetTex_copy,prsc:0,ntxv:0,isnm:False|UVIN-4871-OUT,TEX-8991-TEX;n:type:ShaderForge.SFN_Vector1,id:8507,x:29795,y:35369,varname:node_8507,prsc:0,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:1346,x:32321,y:34598,varname:_node_8258,prsc:0,ntxv:0,isnm:False|UVIN-6691-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Slider,id:3746,x:29737,y:35528,ptovrint:False,ptlb:Offset2,ptin:_Offset2,varname:_Offset2,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.5,cur:0,max:0.5;n:type:ShaderForge.SFN_TexCoord,id:5616,x:31024,y:34946,varname:node_5616,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:6234,x:32334,y:34924,varname:_node_652,prsc:0,ntxv:0,isnm:False|UVIN-2097-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_TexCoord,id:2950,x:31024,y:35618,varname:node_2950,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:3827,x:32358,y:35402,varname:_node_357,prsc:0,ntxv:0,isnm:False|UVIN-4449-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_TexCoord,id:8008,x:31024,y:35309,varname:node_8008,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:8330,x:32371,y:35728,varname:_node_7978,prsc:0,ntxv:0,isnm:False|UVIN-3169-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Add,id:4449,x:31605,y:35313,varname:node_4449,prsc:0|A-6922-OUT,B-8008-UVOUT,C-1224-OUT;n:type:ShaderForge.SFN_Vector2,id:1224,x:31024,y:35483,varname:node_1224,prsc:0,v1:0.001,v2:0.001;n:type:ShaderForge.SFN_Vector2,id:6914,x:31024,y:35940,varname:node_6914,prsc:0,v1:0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:3169,x:31605,y:35608,varname:node_3169,prsc:0|A-2950-UVOUT,B-9075-OUT,C-6914-OUT;n:type:ShaderForge.SFN_Vector2,id:8767,x:31024,y:34827,varname:node_8767,prsc:0,v1:-0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:2097,x:31609,y:35012,varname:node_2097,prsc:0|A-8767-OUT,B-5616-UVOUT,C-6922-OUT;n:type:ShaderForge.SFN_Add,id:6691,x:31587,y:34692,varname:node_6691,prsc:0|A-1940-OUT,B-2396-UVOUT,C-8395-OUT;n:type:ShaderForge.SFN_Vector2,id:1940,x:31036,y:34403,varname:node_1940,prsc:0,v1:-0.001,v2:0.001;n:type:ShaderForge.SFN_TexCoord,id:3355,x:29302,y:35102,varname:node_3355,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:8991,x:28008,y:36313,ptovrint:False,ptlb:Clouds,ptin:_Clouds,varname:_Clouds,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:4871,x:29571,y:35175,varname:node_4871,prsc:0|A-3355-UVOUT,B-5701-OUT;n:type:ShaderForge.SFN_Vector1,id:5701,x:29412,y:35261,varname:node_5701,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Color,id:6933,x:34932,y:36139,ptovrint:False,ptlb:FogColor,ptin:_FogColor,varname:_FogColor,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:3268,x:34824,y:36438,ptovrint:False,ptlb:FogIntensity,ptin:_FogIntensity,varname:_FogIntensity,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:8821,x:35258,y:36337,varname:node_8821,prsc:2|A-3744-OUT,B-6933-RGB,T-3268-OUT;n:type:ShaderForge.SFN_Tex2d,id:1736,x:29977,y:37141,varname:_node_4109,prsc:0,ntxv:0,isnm:False|UVIN-4197-OUT,TEX-8991-TEX;n:type:ShaderForge.SFN_Vector1,id:1640,x:29967,y:37346,varname:node_1640,prsc:0,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:8595,x:32339,y:36551,varname:_node_8097,prsc:0,ntxv:0,isnm:False|UVIN-5499-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Slider,id:2498,x:29905,y:37530,ptovrint:False,ptlb:Offset3,ptin:_Offset3,varname:_Offset3,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.5,cur:0,max:0.5;n:type:ShaderForge.SFN_Tex2d,id:3691,x:32352,y:36877,varname:_node_7615,prsc:0,ntxv:0,isnm:False|UVIN-6284-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:447,x:32376,y:37355,varname:_node_6211,prsc:0,ntxv:0,isnm:False|UVIN-6146-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:9227,x:32389,y:37681,varname:_node_4347,prsc:0,ntxv:0,isnm:False|UVIN-8565-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_TexCoord,id:1201,x:29320,y:37055,varname:node_1201,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4197,x:29589,y:37128,varname:node_4197,prsc:0|A-1201-UVOUT,B-2180-OUT;n:type:ShaderForge.SFN_Vector1,id:2180,x:29430,y:37214,varname:node_2180,prsc:2,v1:-0.25;n:type:ShaderForge.SFN_Tex2d,id:2416,x:29984,y:39083,varname:_node_4109_copy,prsc:0,ntxv:0,isnm:False|UVIN-8001-OUT,TEX-8991-TEX;n:type:ShaderForge.SFN_Vector1,id:833,x:29984,y:39305,varname:node_833,prsc:0,v1:-0.5;n:type:ShaderForge.SFN_Tex2d,id:2307,x:32429,y:38504,varname:_node_8097_copy,prsc:0,ntxv:0,isnm:False|UVIN-3517-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Slider,id:2456,x:30088,y:39486,ptovrint:False,ptlb:Offset4,ptin:_Offset4,varname:_Offset4,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.5,cur:0,max:0.5;n:type:ShaderForge.SFN_Tex2d,id:2255,x:32442,y:38830,varname:_node_7615_copy,prsc:0,ntxv:0,isnm:False|UVIN-2717-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:1983,x:32466,y:39308,varname:_node_6211_copy,prsc:0,ntxv:0,isnm:False|UVIN-2822-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_Tex2d,id:1244,x:32479,y:39634,varname:_node_4347_copy,prsc:0,ntxv:0,isnm:False|UVIN-2515-OUT,TEX-3591-TEX;n:type:ShaderForge.SFN_TexCoord,id:7430,x:29410,y:39008,varname:node_7430,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:8001,x:29679,y:39081,varname:node_8001,prsc:0|A-7430-UVOUT,B-1056-OUT;n:type:ShaderForge.SFN_Vector1,id:1056,x:29520,y:39167,varname:node_1056,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:6463,x:33218,y:34050,varname:node_6463,prsc:2|A-2855-RGB,B-5905-RGB,C-9527-RGB,D-2299-RGB;n:type:ShaderForge.SFN_Add,id:1541,x:33749,y:34598,varname:node_1541,prsc:2|A-6463-OUT,B-1346-RGB,C-6234-RGB,D-3827-RGB,E-8330-RGB;n:type:ShaderForge.SFN_Add,id:1798,x:33597,y:37941,varname:node_1798,prsc:2|A-2307-RGB,B-2255-RGB,C-1983-RGB,D-1244-RGB;n:type:ShaderForge.SFN_Add,id:7118,x:33766,y:36854,varname:node_7118,prsc:2|A-8595-RGB,B-3691-RGB,C-447-RGB,D-9227-RGB,E-1798-OUT;n:type:ShaderForge.SFN_Add,id:865,x:34153,y:35880,varname:node_865,prsc:2|A-1541-OUT,B-7118-OUT;n:type:ShaderForge.SFN_Multiply,id:3744,x:34446,y:35946,varname:node_3744,prsc:2|A-865-OUT,B-9373-OUT;n:type:ShaderForge.SFN_Vector1,id:9373,x:34165,y:36089,varname:node_9373,prsc:2,v1:0.0625;n:type:ShaderForge.SFN_Add,id:5107,x:30063,y:33313,varname:node_5107,prsc:0|A-7014-RGB,B-8085-OUT;n:type:ShaderForge.SFN_Add,id:6645,x:30065,y:35266,varname:node_6645,prsc:2|A-7584-RGB,B-8507-OUT;n:type:ShaderForge.SFN_Multiply,id:4242,x:30304,y:35401,varname:node_4242,prsc:2|A-6645-OUT,B-3746-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2841,x:30545,y:35401,varname:node_2841,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-4242-OUT;n:type:ShaderForge.SFN_Add,id:6934,x:30230,y:37235,varname:node_6934,prsc:0|A-1736-RGB,B-1640-OUT;n:type:ShaderForge.SFN_Add,id:8338,x:30259,y:39233,varname:node_8338,prsc:2|A-2416-RGB,B-833-OUT;n:type:ShaderForge.SFN_Append,id:8395,x:31036,y:34668,varname:node_8395,prsc:2|A-2841-R,B-2841-G;n:type:ShaderForge.SFN_Append,id:6922,x:31024,y:35101,varname:node_6922,prsc:0|A-2841-G,B-2841-B;n:type:ShaderForge.SFN_Append,id:9075,x:31024,y:35779,varname:node_9075,prsc:2|A-2841-B,B-2841-R;n:type:ShaderForge.SFN_TexCoord,id:7048,x:31110,y:32768,varname:node_7048,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:1352,x:31098,y:33193,varname:node_1352,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:3039,x:31098,y:33865,varname:node_3039,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:648,x:31098,y:33556,varname:node_648,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6576,x:31679,y:33560,varname:node_6576,prsc:0|A-8296-OUT,B-648-UVOUT,C-2529-OUT;n:type:ShaderForge.SFN_Vector2,id:2529,x:31098,y:33730,varname:node_2529,prsc:0,v1:0.001,v2:0.001;n:type:ShaderForge.SFN_Vector2,id:844,x:31098,y:34187,varname:node_844,prsc:0,v1:0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:5309,x:31679,y:33855,varname:node_5309,prsc:0|A-3039-UVOUT,B-9936-OUT,C-844-OUT;n:type:ShaderForge.SFN_Vector2,id:9276,x:31098,y:33074,varname:node_9276,prsc:0,v1:-0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:2831,x:31683,y:33259,varname:node_2831,prsc:0|A-9276-OUT,B-1352-UVOUT,C-8296-OUT;n:type:ShaderForge.SFN_Add,id:3880,x:31661,y:32939,varname:node_3880,prsc:0|A-9318-OUT,B-7048-UVOUT,C-4337-OUT;n:type:ShaderForge.SFN_Vector2,id:9318,x:31110,y:32650,varname:node_9318,prsc:0,v1:-0.001,v2:0.001;n:type:ShaderForge.SFN_Multiply,id:2989,x:30329,y:33516,varname:node_2989,prsc:2|A-5107-OUT,B-2077-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2007,x:30574,y:33516,varname:node_2007,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-2989-OUT;n:type:ShaderForge.SFN_Append,id:4337,x:31110,y:32915,varname:node_4337,prsc:2|A-2007-R,B-2007-G;n:type:ShaderForge.SFN_Append,id:8296,x:31098,y:33348,varname:node_8296,prsc:0|A-2007-G,B-2007-B;n:type:ShaderForge.SFN_Append,id:9936,x:31098,y:34026,varname:node_9936,prsc:2|A-2007-B,B-2007-R;n:type:ShaderForge.SFN_TexCoord,id:4682,x:31244,y:36367,varname:node_4682,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:6004,x:31232,y:36792,varname:node_6004,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:5067,x:31232,y:37464,varname:node_5067,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:7650,x:31232,y:37155,varname:node_7650,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6146,x:31813,y:37159,varname:node_6146,prsc:0|A-8781-OUT,B-7650-UVOUT,C-5086-OUT;n:type:ShaderForge.SFN_Vector2,id:5086,x:31232,y:37329,varname:node_5086,prsc:0,v1:0.001,v2:0.001;n:type:ShaderForge.SFN_Vector2,id:5351,x:31232,y:37786,varname:node_5351,prsc:0,v1:0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:8565,x:31813,y:37454,varname:node_8565,prsc:0|A-5067-UVOUT,B-5922-OUT,C-5351-OUT;n:type:ShaderForge.SFN_Vector2,id:45,x:31232,y:36673,varname:node_45,prsc:0,v1:-0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:6284,x:31817,y:36858,varname:node_6284,prsc:0|A-45-OUT,B-6004-UVOUT,C-8781-OUT;n:type:ShaderForge.SFN_Add,id:5499,x:31795,y:36538,varname:node_5499,prsc:0|A-131-OUT,B-4682-UVOUT,C-3411-OUT;n:type:ShaderForge.SFN_Vector2,id:131,x:31244,y:36249,varname:node_131,prsc:0,v1:-0.001,v2:0.001;n:type:ShaderForge.SFN_Multiply,id:517,x:30512,y:37247,varname:node_517,prsc:2|A-6934-OUT,B-2498-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4324,x:30753,y:37247,varname:node_4324,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-517-OUT;n:type:ShaderForge.SFN_Append,id:3411,x:31244,y:36514,varname:node_3411,prsc:2|A-4324-R,B-4324-G;n:type:ShaderForge.SFN_Append,id:8781,x:31232,y:36947,varname:node_8781,prsc:0|A-4324-G,B-4324-B;n:type:ShaderForge.SFN_Append,id:5922,x:31232,y:37625,varname:node_5922,prsc:2|A-4324-B,B-4324-R;n:type:ShaderForge.SFN_TexCoord,id:9596,x:31281,y:38412,varname:node_9596,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:1413,x:31269,y:38837,varname:node_1413,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:699,x:31269,y:39509,varname:node_699,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:3769,x:31269,y:39200,varname:node_3769,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2822,x:31850,y:39204,varname:node_2822,prsc:0|A-9814-OUT,B-3769-UVOUT,C-6266-OUT;n:type:ShaderForge.SFN_Vector2,id:6266,x:31269,y:39374,varname:node_6266,prsc:0,v1:0.001,v2:0.001;n:type:ShaderForge.SFN_Vector2,id:3094,x:31269,y:39831,varname:node_3094,prsc:0,v1:0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:2515,x:31850,y:39499,varname:node_2515,prsc:0|A-699-UVOUT,B-4688-OUT,C-3094-OUT;n:type:ShaderForge.SFN_Vector2,id:5417,x:31269,y:38718,varname:node_5417,prsc:0,v1:-0.001,v2:-0.001;n:type:ShaderForge.SFN_Add,id:2717,x:31854,y:38903,varname:node_2717,prsc:0|A-5417-OUT,B-1413-UVOUT,C-9814-OUT;n:type:ShaderForge.SFN_Add,id:3517,x:31832,y:38583,varname:node_3517,prsc:0|A-3046-OUT,B-9596-UVOUT,C-8245-OUT;n:type:ShaderForge.SFN_Vector2,id:3046,x:31281,y:38294,varname:node_3046,prsc:0,v1:-0.001,v2:0.001;n:type:ShaderForge.SFN_Multiply,id:8009,x:30549,y:39292,varname:node_8009,prsc:2|A-8338-OUT,B-2456-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5945,x:30790,y:39292,varname:node_5945,prsc:0,cc1:0,cc2:1,cc3:2,cc4:-1|IN-8009-OUT;n:type:ShaderForge.SFN_Append,id:8245,x:31281,y:38559,varname:node_8245,prsc:2|A-5945-R,B-5945-G;n:type:ShaderForge.SFN_Append,id:9814,x:31269,y:38992,varname:node_9814,prsc:0|A-5945-G,B-5945-B;n:type:ShaderForge.SFN_Append,id:4688,x:31269,y:39670,varname:node_4688,prsc:2|A-5945-B,B-5945-R;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:7316,x:32468,y:33365,varname:node_7316,prsc:2|OMIN-7787-OUT,OMAX-4284-OUT;n:type:ShaderForge.SFN_Clamp01,id:5182,x:32675,y:33365,varname:node_5182,prsc:2|IN-7316-OUT;n:type:ShaderForge.SFN_Vector1,id:7787,x:32468,y:33515,varname:node_7787,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4284,x:32468,y:33627,varname:node_4284,prsc:2,v1:1;proporder:3591-8991-2077-3746-2498-2456-6933-3268;pass:END;sub:END;*/

Shader "andy/bgBlurMT" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Clouds ("Clouds", 2D) = "white" {}
        _Offset1 ("Offset1", Range(-0.5, 0.5)) = 0
        _Offset2 ("Offset2", Range(-0.5, 0.5)) = 0
        _Offset3 ("Offset3", Range(-0.5, 0.5)) = 0
        _Offset4 ("Offset4", Range(-0.5, 0.5)) = 0
        _FogColor ("FogColor", Color) = (0.5,0.5,0.5,1)
        _FogIntensity ("FogIntensity", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform fixed _Offset1;
            uniform fixed _Offset2;
            uniform sampler2D _Clouds; uniform float4 _Clouds_ST;
            uniform fixed _Offset3;
            uniform fixed _Offset4;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                fixed4 _OffsetTex = tex2D(_Clouds,TRANSFORM_TEX(i.uv0, _Clouds));
                fixed3 node_2007 = ((_OffsetTex.rgb+(-0.5))*_Offset1).rgb;
                fixed2 node_3880 = (fixed2(-0.001,0.001)+i.uv0+float2(node_2007.r,node_2007.g));
                fixed4 _node_2855 = tex2D(_MainTex,TRANSFORM_TEX(node_3880, _MainTex));
                fixed2 node_8296 = float2(node_2007.g,node_2007.b);
                fixed2 node_2831 = (fixed2(-0.001,-0.001)+i.uv0+node_8296);
                fixed4 _node_2713 = tex2D(_MainTex,TRANSFORM_TEX(node_2831, _MainTex));
                fixed2 node_6576 = (node_8296+i.uv0+fixed2(0.001,0.001));
                fixed4 _node_9641 = tex2D(_MainTex,TRANSFORM_TEX(node_6576, _MainTex));
                fixed2 node_5309 = (i.uv0+float2(node_2007.b,node_2007.r)+fixed2(0.001,-0.001));
                fixed4 _node_7397 = tex2D(_MainTex,TRANSFORM_TEX(node_5309, _MainTex));
                fixed2 node_4871 = (i.uv0+0.5);
                fixed4 _OffsetTex_copy = tex2D(_Clouds,TRANSFORM_TEX(node_4871, _Clouds));
                fixed3 node_2841 = ((_OffsetTex_copy.rgb+(-0.5))*_Offset2).rgb;
                fixed2 node_6691 = (fixed2(-0.001,0.001)+i.uv0+float2(node_2841.r,node_2841.g));
                fixed4 _node_8258 = tex2D(_MainTex,TRANSFORM_TEX(node_6691, _MainTex));
                fixed2 node_6922 = float2(node_2841.g,node_2841.b);
                fixed2 node_2097 = (fixed2(-0.001,-0.001)+i.uv0+node_6922);
                fixed4 _node_652 = tex2D(_MainTex,TRANSFORM_TEX(node_2097, _MainTex));
                fixed2 node_4449 = (node_6922+i.uv0+fixed2(0.001,0.001));
                fixed4 _node_357 = tex2D(_MainTex,TRANSFORM_TEX(node_4449, _MainTex));
                fixed2 node_3169 = (i.uv0+float2(node_2841.b,node_2841.r)+fixed2(0.001,-0.001));
                fixed4 _node_7978 = tex2D(_MainTex,TRANSFORM_TEX(node_3169, _MainTex));
                fixed2 node_4197 = (i.uv0+(-0.25));
                fixed4 _node_4109 = tex2D(_Clouds,TRANSFORM_TEX(node_4197, _Clouds));
                fixed3 node_4324 = ((_node_4109.rgb+(-0.5))*_Offset3).rgb;
                fixed2 node_5499 = (fixed2(-0.001,0.001)+i.uv0+float2(node_4324.r,node_4324.g));
                fixed4 _node_8097 = tex2D(_MainTex,TRANSFORM_TEX(node_5499, _MainTex));
                fixed2 node_8781 = float2(node_4324.g,node_4324.b);
                fixed2 node_6284 = (fixed2(-0.001,-0.001)+i.uv0+node_8781);
                fixed4 _node_7615 = tex2D(_MainTex,TRANSFORM_TEX(node_6284, _MainTex));
                fixed2 node_6146 = (node_8781+i.uv0+fixed2(0.001,0.001));
                fixed4 _node_6211 = tex2D(_MainTex,TRANSFORM_TEX(node_6146, _MainTex));
                fixed2 node_8565 = (i.uv0+float2(node_4324.b,node_4324.r)+fixed2(0.001,-0.001));
                fixed4 _node_4347 = tex2D(_MainTex,TRANSFORM_TEX(node_8565, _MainTex));
                fixed2 node_8001 = (i.uv0+0.25);
                fixed4 _node_4109_copy = tex2D(_Clouds,TRANSFORM_TEX(node_8001, _Clouds));
                fixed3 node_5945 = ((_node_4109_copy.rgb+(-0.5))*_Offset4).rgb;
                fixed2 node_3517 = (fixed2(-0.001,0.001)+i.uv0+float2(node_5945.r,node_5945.g));
                fixed4 _node_8097_copy = tex2D(_MainTex,TRANSFORM_TEX(node_3517, _MainTex));
                fixed2 node_9814 = float2(node_5945.g,node_5945.b);
                fixed2 node_2717 = (fixed2(-0.001,-0.001)+i.uv0+node_9814);
                fixed4 _node_7615_copy = tex2D(_MainTex,TRANSFORM_TEX(node_2717, _MainTex));
                fixed2 node_2822 = (node_9814+i.uv0+fixed2(0.001,0.001));
                fixed4 _node_6211_copy = tex2D(_MainTex,TRANSFORM_TEX(node_2822, _MainTex));
                fixed2 node_2515 = (i.uv0+float2(node_5945.b,node_5945.r)+fixed2(0.001,-0.001));
                fixed4 _node_4347_copy = tex2D(_MainTex,TRANSFORM_TEX(node_2515, _MainTex));
                float3 node_3744 = ((((_node_2855.rgb+_node_2713.rgb+_node_9641.rgb+_node_7397.rgb)+_node_8258.rgb+_node_652.rgb+_node_357.rgb+_node_7978.rgb)+(_node_8097.rgb+_node_7615.rgb+_node_6211.rgb+_node_4347.rgb+(_node_8097_copy.rgb+_node_7615_copy.rgb+_node_6211_copy.rgb+_node_4347_copy.rgb)))*0.0625);
                float3 emissive = node_3744;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
