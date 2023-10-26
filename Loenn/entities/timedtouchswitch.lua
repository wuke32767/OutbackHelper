local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local logging = require("logging")
local tts = {}

tts.name = "outback/timedtouchswitch"
tts.depth = -9999
tts.justification = {0.5, 0.5}
tts.placements = {}

local times = {
    ["Slow"] = 15,
    ["Medium"] = 10,
    ["Fast"] = 5
}

for timeName, time in pairs(times) do
    table.insert(tts.placements, {
        name = timeName,
        data = {
            ["startDisappearTime"] = time
        }
    })
end

tts.fieldInformation = {
    startDisappearTime = {
        fieldType = "number",
        minimumValue = 0
    }
}

local containerTexture = "collectables/outback/timedtouchswitch/cont"
local iconTexture = "collectables/outback/timedtouchswitch/idle00"

function tts.rectangle(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    return utils.rectangle(x - 7, y - 7, 14, 14)
end

function tts.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local time = tonumber(entity["startDisappearTime"]) or 15

    local containerSprite = drawableSprite.fromTexture(containerTexture, entity)
    local iconSprite = drawableSprite.fromTexture(iconTexture, entity)

    if time > 10 then
        containerSprite:setColor({0.3, 1.0, 0.3})
        iconSprite:setColor({0.3, 1.0, 0.3})
    elseif time <= 10 and time > 5 then
        containerSprite:setColor({1.0, 1.0, 0.3})
        iconSprite:setColor({1.0, 1.0, 0.3})
    else
        containerSprite:setColor({1.0, 0.3, 0.3})
        iconSprite:setColor({1.0, 0.3, 0.3})
    end
    return {containerSprite, iconSprite}
end

return tts