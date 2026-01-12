mergeInto(LibraryManager.library, {
    ExitGameWeb: function () {
        console.log("ExitGame called from Unity");
        window.location.href = window.location.href;
    }
});