
let lastPanX = 0;
let lastPanY = 0;



window.touchHandler = {
    register: function (element, dotNetHelper) {
        let lastDistance = null;
        let isPinching = false;

        console.log("touchHandler");

        if (typeof DeviceMotionEvent.requestPermission === 'function') {
            DeviceMotionEvent.requestPermission()
                .then(permissionState => {
                    if (permissionState === 'granted') {
                        // adicionar o listener aqui

                        element.addEventListener('devicemotion', function (event) {
                            console.log("devicemotion");
                            const acceleration = event.accelerationIncludingGravity;

                            if (!acceleration) return;

                            const deltaX = acceleration.x || 0;
                            const deltaY = acceleration.y || 0;

                            // Suavização
                            lastPanX += deltaX * 2;
                            lastPanY += deltaY * 2;

                            dotNetHelper.invokeMethodAsync('OnAccelerometerChanged', lastPanX, lastPanY);
                        });
                    }
                })
                .catch(console.error);
        }

        //window.addEventListener('devicemotion', function (event) {
        //    const acceleration = event.accelerationIncludingGravity;

        //    if (!acceleration) return;

        //    const deltaX = acceleration.x || 0;
        //    const deltaY = acceleration.y || 0;

        //    // Suavização
        //    lastPanX += deltaX * 2;
        //    lastPanY += deltaY * 2;

        //    dotNetHelper.invokeMethodAsync('OnAccelerometerChanged', lastPanX, lastPanY);
        //});


        element.addEventListener('touchmove', (e) => {
        });

        element.addEventListener('touchstart', (e) => {
        }, { passive: false });

        element.addEventListener('touchend', (e) => {
           
            dotNetHelper.invokeMethodAsync('OnTouchEnd', JSON.stringify(getTouches(e)));
        }, { passive: false });

        element.addEventListener('click', (e) => {
            console.log("click");
            if (isPinching) {
                e.preventDefault();
                e.stopImmediatePropagation();
                console.log("Clique ignorado por gesto de pinch.");
            }
            else {
                dotNetHelper.invokeMethodAsync('OnClick', JSON.stringify({
                    x: e.clientX,
                    y: e.clientY,
                    button: e.button
                }));
            }

        });
        
        }
    }
}







var s = {
    canvases: {},
    tracked: {},
    getBoundingClientRect: el => {
        return el.getBoundingClientRect();
    },
    mo: new MutationObserver(() => {
        for (id in s.canvases) {
            const canvas = s.canvases[id];
            const lastBounds = canvas.lastBounds;
            const bounds = canvas.elem.getBoundingClientRect();
            if (lastBounds.left !== bounds.left || lastBounds.top !== bounds.top || lastBounds.width !== bounds.width ||
                lastBounds.height !== bounds.height) {
                canvas.lastBounds = bounds;
                canvas.ref.invokeMethodAsync('OnResize', bounds);
            }
        }
    }),
    ro: new ResizeObserver(entries => {
        for (const entry of entries) {
            let id = Array.from(entry.target.attributes).find(e => e.name.startsWith('_bl')).name.substring(4);
            let element = s.tracked[id];
            if (element) {
                element.ref.invokeMethodAsync('OnResize', entry.target.getBoundingClientRect());
            }
        }
    }),
    observe: (element, ref, id) => {
        if (!element) return;
        s.ro.observe(element);
        s.tracked[id] = {
            ref: ref
        };
        if (element.classList.contains("diagram-canvas")) {
            s.canvases[id] = {
                elem: element,
                ref: ref,
                lastBounds: element.getBoundingClientRect()
            };
        }
    },
    unobserve: (element, id) => {
        if (element) {
            s.ro.unobserve(element);
        }
        delete s.tracked[id];
        delete s.canvases[id];
    }
};
window.ZBlazorDiagrams = s;



//window.addEventListener('devicemotion', function (event) {
//    const acceleration = event.accelerationIncludingGravity;

//    if (!acceleration) return;

//    const deltaX = acceleration.x || 0;
//    const deltaY = acceleration.y || 0;

//    // Suavização
//    lastPanX += deltaX * 2;
//    lastPanY += deltaY * 2;

//    dotNetHelper.invokeMethodAsync('OnAccelerometerChanged', lastPanX, lastPanY);
//});

window.addEventListener('scroll', () => {
    for (id in s.canvases) {
        const canvas = s.canvases[id];
        canvas.lastBounds = canvas.elem.getBoundingClientRect();
        canvas.ref.invokeMethodAsync('OnResize', canvas.lastBounds);
    }
});
s.mo.observe(document.body, {childList: true, subtree: true});