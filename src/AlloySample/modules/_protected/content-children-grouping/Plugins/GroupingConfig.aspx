﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupingConfig.aspx.cs" Inherits="ContentChildrenGrouping.Plugin.GroupingConfig" %>

<asp:Content runat="server" ContentPlaceHolderID="HeaderContentRegion">
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width,initial-scale=1"/>
    <meta name="theme-color" content="#000000"/>
    <link href='<%= GetPath("admin.css") %>' rel="stylesheet" />
    <title>Group containers configuration</title>
    <link href='<%= GetPath("static/css/2.chunk.css") %>' rel="stylesheet" />
    <link href='<%= GetPath("static/css/main.chunk.css") %>'rel="stylesheet" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainRegion">
    <noscript>You need to enable JavaScript to run this app.</noscript>
    
    <div class="epi-formArea">
        <div class="epi-size20">
           <div id="root" data-configuration="<%: Configuration %>"></div>
           <script>!function (e) { function r(r) { for (var n, a, i = r[0], c = r[1], l = r[2], f = 0, s = []; f < i.length; f++)a = i[f], Object.prototype.hasOwnProperty.call(o, a) && o[a] && s.push(o[a][0]), o[a] = 0; for (n in c) Object.prototype.hasOwnProperty.call(c, n) && (e[n] = c[n]); for (p && p(r); s.length;)s.shift()(); return u.push.apply(u, l || []), t() } function t() { for (var e, r = 0; r < u.length; r++) { for (var t = u[r], n = !0, i = 1; i < t.length; i++) { var c = t[i]; 0 !== o[c] && (n = !1) } n && (u.splice(r--, 1), e = a(a.s = t[0])) } return e } var n = {}, o = { 1: 0 }, u = []; function a(r) { if (n[r]) return n[r].exports; var t = n[r] = { i: r, l: !1, exports: {} }; return e[r].call(t.exports, t, t.exports, a), t.l = !0, t.exports } a.e = function (e) { var r = [], t = o[e]; if (0 !== t) if (t) r.push(t[2]); else { var n = new Promise((function (r, n) { t = o[e] = [r, n] })); r.push(t[2] = n); var u, i = document.createElement("script"); i.charset = "utf-8", i.timeout = 120, a.nc && i.setAttribute("nonce", a.nc), i.src = function (e) { return a.p + "static/js/" + ({}[e] || e) + ".chunk.js" }(e); var c = new Error; u = function (r) { i.onerror = i.onload = null, clearTimeout(l); var t = o[e]; if (0 !== t) { if (t) { var n = r && ("load" === r.type ? "missing" : r.type), u = r && r.target && r.target.src; c.message = "Loading chunk " + e + " failed.\n(" + n + ": " + u + ")", c.name = "ChunkLoadError", c.type = n, c.request = u, t[1](c) } o[e] = void 0 } }; var l = setTimeout((function () { u({ type: "timeout", target: i }) }), 12e4); i.onerror = i.onload = u, document.head.appendChild(i) } return Promise.all(r) }, a.m = e, a.c = n, a.d = function (e, r, t) { a.o(e, r) || Object.defineProperty(e, r, { enumerable: !0, get: t }) }, a.r = function (e) { "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(e, Symbol.toStringTag, { value: "Module" }), Object.defineProperty(e, "__esModule", { value: !0 }) }, a.t = function (e, r) { if (1 & r && (e = a(e)), 8 & r) return e; if (4 & r && "object" == typeof e && e && e.__esModule) return e; var t = Object.create(null); if (a.r(t), Object.defineProperty(t, "default", { enumerable: !0, value: e }), 2 & r && "string" != typeof e) for (var n in e) a.d(t, n, function (r) { return e[r] }.bind(null, n)); return t }, a.n = function (e) { var r = e && e.__esModule ? function () { return e.default } : function () { return e }; return a.d(r, "a", r), r }, a.o = function (e, r) { return Object.prototype.hasOwnProperty.call(e, r) }, a.p = "/", a.oe = function (e) { throw console.error(e), e }; var i = this["webpackJsonpmy-app"] = this["webpackJsonpmy-app"] || [], c = i.push.bind(i); i.push = r, i = i.slice(); for (var l = 0; l < i.length; l++)r(i[l]); var p = c; t() }([])</script>
           <script src='<%= GetPath("static/js/2.chunk.js") %>'></script>
            <script src='<%= GetPath("static/js/main.chunk.js") %>'></script>
        </div>
    </div>
    
    <script type="text/javascript">
        var helpLink = document.querySelector("h1.EP-prefix a");
        helpLink.href = "https://github.com/gregwiechec/content-children-grouping";
        helpLink.onclick = function() { return true; }
    </script>
</asp:Content>