local drawableSprite = require("structs.drawable_sprite")
local mts = {}

mts.name = "outback/movingtouchswitch"
mts.depth = -9999
mts.justification = {0.5, 0.5}
mts.nodeLimits = {0, -1}
mts.nodeLineRenderType = "line"
mts.placements = {
    name = "MovingTouchSwitch"
}

local containerTexture = "collectables/outback/movingtouchswitch/container"
local iconTexture = "collectables/outback/movingtouchswitch/icon00"

function mts.sprite(room, entity)
    local containerSprite = drawableSprite.fromTexture(containerTexture, entity)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)

    return {containerSprite, iconSprite}
end

function mts.nodeSprite(room, entity, node)
    local containerSprite = drawableSprite.fromTexture(containerTexture, entity)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)

    containerSprite:setPosition(node.x, node.y)
    iconSprite:setPosition(node.x, node.y)

    return {containerSprite, iconSprite}
end

return mts